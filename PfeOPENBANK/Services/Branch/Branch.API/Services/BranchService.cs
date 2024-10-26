using Branch.API.Models;
using Branch.API.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;
using Branch.API.Models.Response;
using Branch.API.Models.Requests;
using Branch.API.Extensions;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Branch.API.Services
{
    public class BranchService : IBranchService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<BranchService> _logger;
        private readonly HttpClient _client;
        private string endPointUrl = "";

        public BranchService(IConfiguration configuration, ILogger<BranchService> logger, HttpClient httpClient) {

            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Setting endPoint
            endPointUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseTableName")}/";
        }

        public async Task<BranchResponse> AddBranch(BranchRequest branch)
        {
            Random rand = new Random();
            _logger.LogInformation("Addition of a branch in progress...");
                BranchModel model = new BranchModel() { BRANCH_TYPE = branch.Branch_type, BANK_ID = branch.Bank_id,
                NAME = branch.Name, LINE_1 = branch.Adresse.Line_1, LINE_2 = branch.Adresse.Line_2, 
                    LINE_3 = branch.Adresse.Line_3,
                   CITY = branch.Adresse.City, STATE = branch.Adresse.State, COUNTRY= branch.Adresse.Country, 
                    POSTCODE = branch.Adresse.Postcode,COUNTRY_CODE = branch.Adresse.Country_code,
                    LATITUDE = branch.Location.Latitude, LONGITUDE = branch.Location.Longitude,
                    LICENCE_ID = branch.Meta.Licence.Id, LICENCE_NAME = branch.Meta.Licence.Name, 
                    MORE_INFO = branch.More_info, CREATEDAT = DateTime.Now,
                UPDATEDAT = DateTime.Now, ACCESSIBLE_FEATURES = branch.AccessibleFeatures,
                PHONE_NUMBER = branch.Phone_number, ROUTING_ADDRESS = branch.Branch_routing.Address, 
                    ROUTING_SCHEME = branch.Branch_routing.Scheme,
                ACCESSIBLE_VALUE = branch.Is_accessible, HAS_LOBBY = branch.Lobby, HAS_DRIVE_UP = branch.Drive_up,
                ON_MONDAY = branch.Monday, ON_TUESDAY = branch.Tuesday, ON_THURSDAY = branch.Thursday, 
                    ON_WEDNESDAY = branch.Wednesday,
                ON_FRIDAY = branch.Friday, ON_SATURDAY = branch.Saturday, ON_SUNDAY = branch.Sunday};

            model.CLOSING_TIME = DateTimeExtension.SetTime(DateTime.Now, branch.Closing_time.Hour, branch.Closing_time.Minute,0,0);
             
            model.OPENING_TIME = DateTimeExtension.SetTime(DateTime.Now, branch.Opening_time.Hour, branch.Opening_time.Minute,0,0);

            model.ID = Guid.NewGuid().ToString();
            model.BRANCH_NUM = rand.NextInt64(100000000000);

            var branchPost = JsonConvert.SerializeObject(model);
            var response = await _client.PostAsync(endPointUrl, new StringContent(branchPost, Encoding.UTF8, "application/json"));
            try
            {
                response.EnsureSuccessStatusCode();
                model = await response.Content.ReadAsAsync<BranchModel>();
                return GetBranchResponseFromModel(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new BranchResponse() { Code = 500, ErrorMessage = "OBP-50000: Unknown Error." };
            }
            
        }

        public async Task<bool> DeleteBranch(string branch_id)
        {
            var response = await _client.DeleteAsync(endPointUrl + branch_id);
            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<BranchListResponse> GetAllBranches(string bank_id,int? page, int? size)
        {
            try
            {
                string filter = "{ \"bank_id\": { \"$eq\" :\"" + bank_id + "\" } }";
                    ;
                var result = await _client.GetAsync($"{endPointUrl}?q={filter}&offet={page}&limit={size}");

                BranchList listModel = await result.Content.ReadAsAsync<BranchList>();
                BranchListResponse list = new BranchListResponse();
                list.Offset = listModel.Offset;
                list.Count = listModel.Count;
                list.Limit = listModel.Limit;
                list.HasMore = list.HasMore;
                foreach( var item in listModel.Items )
                {
                    list.Branches.Add( GetBranchResponseFromModel(item) );
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BranchListResponse();
            }
        }

        public async Task<BranchResponse> GetBranch(string id)
        {
            var result = await _client.GetAsync(endPointUrl + id);

            try
            {
                BranchModel model = await result.Content.ReadAsAsync<BranchModel>();
                return GetBranchResponseFromModel(model);
            }
            catch (Exception ex)
            {
                BranchResponse resp =  new BranchResponse() { Code = (int)result.StatusCode };
                resp.ErrorMessage = resp.Code == 404 ? "OBP-300010: Branch not found. Please specify a valid value for BRANCH_ID. Or License may not be set. meta.license.id and meta.license.name can not be empty" : "OBP-50000: Unknown Error.";
                return resp;
            }
        }

        public async Task<BranchListResponse> GetBranchesByFilter(string filter, string bank_id,int? page, int? size)
        {
            try
            {
                var filterJson = JsonConvert.DeserializeObject(filter);
                var result = await _client.GetAsync($"{endPointUrl}?q={filter}&offet={page}&limit={size}");

                BranchList listModel = await result.Content.ReadAsAsync<BranchList>();
                BranchListResponse list = new BranchListResponse();
                list.Offset = listModel.Offset;
                list.Count = listModel.Count;
                list.Limit = listModel.Limit;
                list.HasMore = list.HasMore;
                foreach (var item in listModel.Items)
                {
                    list.Branches.Add(GetBranchResponseFromModel(item));
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return await GetAllBranches(bank_id,1,10);
            }
        }

        public async Task<bool> UpdateBranch(BranchModel branch)
        {
            var result = await _client.GetAsync(endPointUrl + branch.ID);

            result.EnsureSuccessStatusCode();
            var exitedBranch = await result.Content.ReadAsAsync<BranchModel>();
            branch.CREATEDAT = exitedBranch.CREATEDAT;
            branch.UPDATEDAT = DateTime.Now;
            try
            {
                var branchPost = JsonConvert.SerializeObject(branch);

                var response = _client.PutAsync(endPointUrl + branch.ID, new StringContent(branchPost, Encoding.UTF8, "application/json"));

                await response.Result.Content.ReadAsAsync<BranchModel>();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Updating branch failed! "+ex.Message);
                return false;
            }
        }

        private BranchResponse GetBranchResponseFromModel(BranchModel model)
        {
            BranchResponse response = new BranchResponse();
            response.Id = model.ID;
            response.Bank_id = model.BANK_ID;
            response.Name = model.NAME;
            response.Address = new AddressModel () { Line_1 = model.LINE_1, Line_2 = model.LINE_2, Line_3 = model.LINE_3,
            City = model.CITY, Country = model.COUNTRY, Country_code = model.COUNTRY_CODE, Postcode = model.POSTCODE, State = model.STATE};
            response.Location = new LocationModel() { Latitude = model.LATITUDE, Longitude = model.LONGITUDE };
            response.Meta = new MetaModel() { Licence = new LicenceModel() { Id = model.LICENCE_ID, Name = model.LICENCE_NAME } };
            response.Branch_routing = new RoutingModel() { Address = model.ROUTING_ADDRESS, Scheme = model.ROUTING_SCHEME };
            response.Is_accessible = model.ACCESSIBLE_VALUE;
            response.AccessibleFeatures = model.ACCESSIBLE_FEATURES;
            response.Branch_type = model.BRANCH_TYPE;
            response.More_info = model.MORE_INFO;
            response.Phone_Number = model.PHONE_NUMBER;

            WorkingTimeModel workingTime = new WorkingTimeModel() { Opening_time = model.OPENING_TIME.TimeOfDay
                , Closing_time = model.CLOSING_TIME.TimeOfDay };
            
            LobbyModel lobby = new LobbyModel();
            DriveUpModel driveUp = new DriveUpModel();

            if (model.ON_MONDAY)
            {
                lobby.Monday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Monday = workingTime;
                }
            }
            if (model.ON_TUESDAY)
            {
                lobby.Tuesday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Tuesday = workingTime;
                }
            }
            if (model.ON_WEDNESDAY)
            {
                lobby.Wednesday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Wednesday = workingTime;
                }
            }
            if (model.ON_THURSDAY)
            {
                lobby.Thursday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Thursday = workingTime;
                }
            }
            if (model.ON_FRIDAY)
            {
                lobby.Friday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Friday = workingTime;
                }
            }
            if (model.ON_SATURDAY)
            {
                lobby.Saturday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Saturday = workingTime;
                }
            }
            if (model.ON_SUNDAY)
            {
                lobby.Sunday.Add(workingTime);
                if (model.HAS_DRIVE_UP)
                {
                    driveUp.Sunday = workingTime;
                }
            }
            response.Lobby = lobby;
            response.Drive_up = driveUp;
            return response;
        }
    }
}

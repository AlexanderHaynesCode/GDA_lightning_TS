using Microsoft.AspNetCore.Mvc;  
using GDA_lightning_TS.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage;
using Azure.Storage.Blobs.Models;

namespace GDA_lightning_TS.Controllers  
{    
    public class UserController : ControllerBase
    {  
        UserDataAccessLayer objuser = new UserDataAccessLayer(); 
        ILogger<UserController> _logger;
        

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }         

        [HttpGet]  
        [Route("user/GetAll")]  
        public IEnumerable<User_GDA> GetAll()  
        {  
            return objuser.GetAllUsers();  
        }  

        [HttpGet]  
        [Route("user/GetAllExceptCurrent/{currentUserID}")]  
        public IEnumerable<User_GDA> GetAllExceptCurrent(int currentUserID)  
        {  
            return objuser.GetAllExceptCurrent(currentUserID);  
        }   

        [HttpGet]  
        [Route("user/GetOne/{userID}")]  
        public User_GDA GetOne(int userID)  
        {  
            return objuser.GetOne(userID);  
        }   

        [HttpGet]  
        [Route("user/GetTwo/{fromID}/{toID}")]  
        public IEnumerable<User_GDA> GetTwo(int fromID, int toID)  
        {  
            return objuser.GetTwo(fromID, toID);  
        } 

        [HttpGet]  
        [Route("user/DeleteTheTestAutomationAccounts")]  
        public int DeleteTheTestAutomationAccounts()  
        {  
            return objuser.DeleteTheTestAutomationAccounts();  
        }  

        [HttpPost]  
        [Route("user/blacklisted_user_attempted_login/{userId}")]  
        public int blacklisted_user_attempted_login()  
        {  
            return 1;  // To-do: get this fully functional
        }
        

        [HttpGet]  
        [Route("user/GetFilteredSearchResults/{currentUserID}/{lookingFor}")]  
        public IEnumerable<User_GDA> GetFilteredSearchResults(int currentUserID, string lookingFor)  
        {  
            IEnumerable<User_GDA> ret = objuser.GetFilteredSearchResults(currentUserID, lookingFor); 
            return ret;
        } 

        [HttpGet]  
        [Route("user/GetUserIDByUsername/{username}")]  
        public int GetUserIDByUsername(string username)  
        {  
            return objuser.GetUserIDByUsername(username);  
        }

        [HttpGet]  
        [Route("user/SignIn/{Username}/{password}")]  
        public int SignIn(string username, string password)  
        {  
            var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return objuser.SignIn(username, password, ip);  
        }

        [HttpGet]
        [Route("user/SignIn2/{Username}/{password}")]
        public User_GDA_Global SignIn2(string username, string password)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = objuser.SignIn2(username, password, ip);
            if (result == null) {
                var emptyUser = new User_GDA_Global();
                return emptyUser;
            } else {
                return result;
            }            
        }

        [HttpGet]  
        [Route("user/CheckLoggedIn")]  
        public int CheckLoggedIn()  
        {  
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            if (ip != null) {                
                Console.WriteLine("ip: " + ip.ToString());
                System.Diagnostics.Debug.WriteLine("ipp: " + ip.ToString());
                return objuser.CheckIfLoggedIn(ip.ToString());
            } else {
                return 0;
            }
        }        

        [HttpGet]  
        [Route("user/GetIP")]  
        public string GetIP()  
        {  
            return "1277777";
            // try {
            //     var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            //     if (ip == null) {
            //         return "IP is null.";
            //     } else if (ip == "::1") {
            //         return "a::1";
            //     }
            //     return ip;
            // } catch (Exception e) {
            //     return "Error getting IP.";
            // }            
        }  

        [HttpGet]  
        [Route("user/GetConversationsByUser/{currentUserID}")]  
        public IEnumerable<Conversation_GDA> GetConversationsByUser(int currentUserID)  
        {  
            return objuser.GetConversationsByUser(currentUserID);  
        }   

        [HttpGet]   
        [Route("user/GetConversationsByUserWithUsernames/{currentUserID}")]  
        public ConversationWithUnread_GDA[] GetConversationsByUserWithUsernames(int currentUserID)  
        {  
            return objuser.GetConversationsByUserWithUsernames(currentUserID);  
        } 

        
        [HttpGet]   
        [Route("user/CheckForUnreadMessages/{UserID}")]  
        public int CheckForUnreadMessages(int UserID)  
        {  
            return objuser.CheckForUnreadMessages(UserID);  
        }

        [HttpGet]   
        [Route("user/GetMessagesByConversation/{conversation_ID}/{currUserID}")]  
        public IEnumerable<Message_GDA> GetMessagesByConversation(int conversation_ID, int currUserID)  
        {  
            return objuser.GetMessagesByConversation(conversation_ID, currUserID);  
        }
           
        [HttpPost]  
        [Route("user/create")]  
        public int Create(User_GDA user)
        {  
            user.DateCreated = DateTime.Now;
            string ip = GetIPAddress();
            return (int)objuser.AddUser(user, ip);             
        }  

        [HttpPut]  
        [Route("user/SignOut")]  
        public int SignOut(int userID)
        {  
            return objuser.SignOut(userID);  
        }
        
        [HttpPost]
        [Route("user/CreateForSASUri")]
        public async Task<int> CreateForSASUri(UserForSASUrl_GDA userForSAS)
        {
            var user = new User_GDA(userForSAS);
            var filename = userForSAS.ProfilePicFileName ?? "";

            if (userForSAS.ProfilePicFile != null && !string.IsNullOrEmpty(filename))
            {
                var uploadResult = await UploadProfilePictureAsync(userForSAS.ProfilePicFile, filename);
                if (uploadResult)
                {
                    var sasUri = await CreateSASUriAsync(filename);
                    user.ProfilePicPath = sasUri; // Only set if SAS URI was created successfully
                }
            }

            return Create(user);
        }

        private async Task<string> CreateSASUriAsync(string filename)
        {
            filename = $"images/{filename}";
            var blobClient = await GetBlobClientAsync(filename);
            
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // More secure to have shorter-lived SAS
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }
            return string.Empty;
        }

        private async Task<bool> UploadProfilePictureAsync(IFormFile file, string filename)
        {
            try
            {
                // TO-DO: implement image compression if needed
                
                var blobClient = await GetBlobClientAsync(filename, "images");
                await blobClient.UploadAsync(file.OpenReadStream(), new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType } });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error uploading profile picture: {ex.Message}");
                return false;
            }
        }

        // Helper method to reduce code duplication
        private async Task<BlobClient> GetBlobClientAsync(string filename, string prefix = "")
        {
            var containerClient = await GetContainerClientAsync();
            return containerClient.GetBlobClient($"{prefix}/{filename}");
        }

        // Helper method to reduce code duplication
        private Task<BlobContainerClient> GetContainerClientAsync()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var accountName = MyConfig.GetValue<string>("ConnectionStrings:AZURE_STORAGE_ACCOUNT_NAME");
            var accountKey = MyConfig.GetValue<string>("ConnectionStrings:AZURE_STORAGE_ACCOUNT_KEY");
            var containerName = MyConfig.GetValue<string>("ConnectionStrings:AZURE_STORAGE_CONTAINER_NAME");

            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);
            return Task.FromResult(blobServiceClient.GetBlobContainerClient(containerName));
        }

        [HttpPost]  
        [Route("user/StartConversationWithAnotherUser")]  
        public int StartConversationWithAnotherUser(string message, int person1_ID, int person2_ID)
        {  
            Conversation_GDA convo = new Conversation_GDA(person1_ID, person2_ID);
            return objuser.StartConversation(convo, message);
        }  

        [HttpPost]  
        [Route("user/SendMessageToAnotherUser")]  
        public int SendMessageToAnotherUser(int convo_ID, int fromID, int toID, string newMessage)
        {
            Message_GDA message = new Message_GDA(convo_ID, fromID, toID, newMessage)
            {
                DateCreated = DateTime.Now
            };
            return objuser.SendMessageToAnotherUser(message);
        }

        public string GetIPAddress()  
        {  
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            if (!(ip == null)) {
                Console.WriteLine("ip: " + ip.ToString());
                return ip.ToString(); 
            } else {
                return null;
            }
        }         
    }  
}



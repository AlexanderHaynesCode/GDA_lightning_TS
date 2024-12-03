using Microsoft.EntityFrameworkCore; 
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace GDA_lightning_TS.Models  
{  
    public class UserDataAccessLayer  
    {        
        CoreDbContext db = new CoreDbContext();  

        public UserDataAccessLayer()
        {           
        }

        public enum Result
        {
            Error = 0,
            Success = 1,
            UsernameTakenError = 2
        }

        public IEnumerable<User_GDA> GetAllUsers()  
        {  
            try  
            {  
                return db.User_GDA.ToList();  
            }  
            catch  (Exception e)
            {  
                Debug.WriteLine("Error in GetAllUsers: " + e.Message);
                throw;  
            }  
        }   

        public IEnumerable<User_GDA> GetAllExceptCurrent(int currentUserID)  
        {  
            try  
            {  
                var results = db.User_GDA.Where(x => x.Id != currentUserID);  
                return results;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in GetAllExceptCurrent: " + e.Message);
                throw;  
            }  
        }

        public User_GDA GetOne(int userID)  
        {  
            try  
            {  
                var user = db.User_GDA.First(x => x.Id == userID);
                return user;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in GetOne: " + e.Message);
                throw;  
            }  
        }   

        public IEnumerable<User_GDA> GetTwo(int fromID, int toID)
        {  
            try  
            {  
                IEnumerable<User_GDA> users = db.User_GDA.Where(x => x.Id == fromID || x.Id == toID);
                return users;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in GetTwo: " + e.Message);
                throw;  
            }  
        } 

        public int DeleteTheTestAutomationAccounts()
        {  
            try  
            {
                // "testuser3" is used in the automation tests to create a new user, so they need
                // to be deleted for it work.
                IEnumerable<User_GDA> users = db.User_GDA.Where(x => x.Username == "test987654321" ||
                                                                x.Username == "test987654322" ||
                                                                x.Username == "testuser3"); 
                
                if (users.Count() == 0) {
                    return 2;  
                }

                foreach (var user in users) {
                    db.User_GDA.Remove(user);
                }
                
                db.SaveChanges();
                return 1;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in DeleteTheTestAutomationAccounts: " + e.Message);
                return 0;  
            }  
        }

        public IEnumerable<User_GDA> GetFilteredSearchResults(int currentUserID, string lookingFor)
        {  
            try  
            {                  
                int LF = 0;
                if (lookingFor == "both") {
                    IEnumerable<User_GDA> users2 = db.User_GDA.Where(x => x.Id != currentUserID);
                    return users2;
                } 
                else if (lookingFor == "none") {
                    IEnumerable<User_GDA> ret = Enumerable.Empty<User_GDA>();
                    return ret;
                }
                else if (lookingFor == "males") {
                    LF = 1;
                } else {
                    LF = 2;
                }
                IEnumerable<User_GDA> users = db.User_GDA.Where(x => x.Id != currentUserID && x.Gender == LF);
                return users;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in GetFilteredSearchResults: " + e.Message);
                throw;  
            }  
        } 

        public string GetCurrentUserLookingForVal(int currentUserID)
        {  
            try  
            {  
                User_GDA user = db.User_GDA.First(x => x.Id == currentUserID);                
                return user.LookingFor;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in GetCurrentUserLookingForVal: " + e.Message);
                return "";  
            }  
        } 

        public int GetUserIDByUsername(string username)
        {  
            try  
            {  
                var user = db.User_GDA.First(x => x.Username == username);
                return user.Id;
            }  
            catch  (Exception e)
            {
                Debug.WriteLine("Error in GetUserIDByUsername: " + e.Message);
                throw;  
            }  
        }
        
        public int CheckForUnreadMessages(int UserID) 
        {
            try
            {
                string sql = "SELECT CASE WHEN EXISTS ( ";
                sql += "    SELECT * FROM Messages_GDA m ";
                sql += "    JOIN (SELECT MAX(t.DateCreated) AS LatestToMsg_user ";
                sql += "    FROM Messages_GDA t WHERE t.Conversation_ID in ";
                sql += "            (select Conversation_ID from Conversations_GDA c where ((Person1_ID = @UserID and Person1_InConvo = 'True') ";
                sql += "            or (Person2_ID = @UserID and Person2_InConvo = 'True'))) ";
                sql += "            and (t.To_ID_Read = 'False') ";
                sql += "        and t.To_ID = @UserID group by t.Conversation_ID) n ";
                sql += "        ON n.LatestToMsg_user = m.DateCreated ";
                sql += "    ) ";
                sql += "    THEN 'TRUE' ";
                sql += "    ELSE 'FALSE' ";
                sql += "END ";
                var pUserID = new SqlParameter("@UserID", UserID);
                var sResults = db.StringObj.FromSqlRaw(sql, pUserID).ToList();

                return sResults[0].Value == "TRUE" ? 1 : 0;
            } 
            catch (Exception e)
            {
                Debug.WriteLine("Error in CheckForUnreadMessages: " + e.Message);
                return 0;
            }            
        }

        public IEnumerable<Conversation_GDA> GetConversationsByUser(int currentUserID)
        {  
            try  
            {  
                var results = db.Conversations_GDA.Where(x => x.Person1_ID == currentUserID || x.Person2_ID == currentUserID).ToList();
                return results;
            }  
            catch   (Exception e)
            {
                Debug.WriteLine("Error in GetConversationsByUser: " + e.Message);
                throw;  
            }  
        }  

        public ConversationWithUnread_GDA[] GetConversationsByUserWithUsernames(int currentUserID)
        {  
            try  
            {  
                ConversationWithUsernames_GDA[] conversations = GetConversationsByUserWithUsernamesSQL(currentUserID);
                ConversationWithUnread_GDA[] conversationsWithUnread = UpdateWithToIDRead(conversations, currentUserID);
                return conversationsWithUnread;
            }  
            catch   (Exception e)
            {
                Debug.WriteLine("Error in GetConversationsByUserWithUsernames: " + e.Message);
                throw;  
            }  
        } 

        public ConversationWithUnread_GDA[] UpdateWithToIDRead(ConversationWithUsernames_GDA[] conversations, int currentUserID)
        {
            try  
            {
                // This query returns all Conversation_IDs in which currentUserID has unread messages.
                string sql = "SELECT Conversation_ID FROM Messages_GDA m "; 
                            sql += "JOIN (SELECT MAX(t.DateCreated) AS LatestToMsg_user ";
                            sql += "   FROM Messages_GDA t "; 
                            sql += "   WHERE t.Conversation_ID in  ";
                            sql += "   (select Conversation_ID from Conversations_GDA c ";
                            sql += "        where ((Person1_ID = @currentUserID and Person1_InConvo = 'True') ";  
                            sql += "            or (Person2_ID = @currentUserID and Person2_InConvo = 'True'))) ";
                            sql += "        and (t.To_ID_Read = 'False') ";
                            sql += "        and t.To_ID = @currentUserID group by t.Conversation_ID) n "; 
                            sql += "   ON n.LatestToMsg_user = m.DateCreated ";
                var pCurrentUserID = new SqlParameter("@currentUserID", currentUserID);  
                var convosWithUnreadMessages = db.IntObject.FromSqlRaw(sql, pCurrentUserID).ToArray();

                // Copy all convos to the new data type that contains the Has_Unread_Message flag.
                int i = 0;
                ConversationWithUnread_GDA[] ret = new ConversationWithUnread_GDA[conversations.Length];
                foreach (var convo in conversations) {
                    ret[i++] = new ConversationWithUnread_GDA(convo);
                }
                
                // Update the convos that have unread messages so they properly display in the UI differently.
                foreach (var convoWithUnreadMessages in convosWithUnreadMessages) {
                    foreach (var convo in ret) {
                        if (convo.Conversation_ID == convoWithUnreadMessages.Value) {
                            convo.Has_Unread_Message = true;
                        }
                    }                   
                }
                
                return ret;
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in UpdateWithToIDRead: " + e.Message);
                throw;
            } 
        } 

        public ConversationWithUsernames_GDA[] GetConversationsByUserWithUsernamesSQL(int currentUserID)
        {  
            try  
            {
                string sql = "SELECT c.*, u.Username AS Person1_Username, u2.Username AS Person2_Username ";
                            sql += "FROM Conversations_GDA c "; 
                            sql += "LEFT JOIN User_GDA u ON c.Person1_ID = u.ID ";
                            sql += "LEFT JOIN User_GDA u2 ON c.Person2_ID = u2.ID "; 
                            sql += "WHERE c.Person1_ID = @currentUserID OR c.Person2_ID = @currentUserID ";
                var pCurrentUserID = new SqlParameter("@currentUserID", currentUserID);  
                var conversations = db.ConversationWithUsernames_GDA.FromSqlRaw(sql, pCurrentUserID).ToArray();
                
                return conversations;                 
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in GetConversationsByUserWithUsernamesSQL: " + e.Message);
                throw;
            }  
        }  

        public IEnumerable<Message_GDA> GetMessagesByConversation(int conversation_ID, int currUserID)
        {  
            try  
            {                
                var messages = db.Messages_GDA.Where(x => x.Conversation_ID == conversation_ID);

                // Update all messages to read where the "to" user is the currentUser.
                var unreadMessages = messages.Where(m => m.To_ID == currUserID && m.To_ID_Read == false);
                foreach (var msg in unreadMessages) {
                    msg.To_ID_Read = true;
                    db.Messages_GDA.Update(msg);    
                }
                db.SaveChanges();
                
                return messages;
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in GetMessagesByConversation: " + e.Message);
                throw;  
            }  
        }

        // If username and password match found then return 1, otherwise 0.
        public int SignIn(string username, string password, string ip)  
        {  
            try  
            {  
                var results = db.User_GDA.Where(x => x.Username == username && x.Password == password);
                if (results.Any()) { 
                    SetExpiresOnValue(results.FirstOrDefault().Id, ip);

                    return results.FirstOrDefault().Id;                  
                } 
                return 0;                                   
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in SignIn: " + e.Message);
                return 0;
            }  
        }

        public User_GDA_Global? SignIn2(string username, string password, string ip)
        {
            try
            {
                var results = db.User_GDA.Where(x => x.Username == username && x.Password == password);
                if (results.Any())
                {
                    SetExpiresOnValue(results.FirstOrDefault().Id, ip);

                    var user_global = new User_GDA_Global(results.FirstOrDefault());

                    return user_global;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("SignIn2 error: " + e);
                return null;
            }
        }                 

        public Result AddUser(User_GDA user, string ip)
        {
            try
            {
                var usernameTaken = db.User_GDA.FirstOrDefault(u => u.Username == user.Username);
                if (usernameTaken != null) {
                    return Result.UsernameTakenError;
                }

                db.User_GDA.Add(user);

                if (ip != null)
                {
                    var userLoggedIn = new UserLoggedIn_GDA(user.Id)
                    {
                        IPAddress = ip
                    };
                    db.UserLoggedIn_GDA.Add(userLoggedIn);

                    var usersOnSameIP = db.UserLoggedIn_GDA.Where(u => u.IPAddress == ip);
                    foreach (var u in usersOnSameIP)
                    {
                        u.ExpiresOn = null;
                        db.UserLoggedIn_GDA.Update(u);
                    }
                }

                db.SaveChanges();
                return Result.Success;
            }
            catch (DbUpdateException e)
            {
                Debug.WriteLine("Error in AddUser DbUpdateException: " + e.Message);
                return Result.Error;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in AddUser: " + e.Message);
                return Result.Error;
            }
        }

        public int AddUser(User_GDA user)  
        {  
            try  
            {  
                db.User_GDA.Add(user); 
                db.SaveChanges();  

                return 1; 
            }  
            catch (Exception e) 
            {
                Debug.WriteLine("Error in AddUser: " + e.Message);
                throw;  
            }              
        } 
        
        public int CheckIfLoggedIn(string ip) 
        {
            try
            {  
                var userLog = db.UserLoggedIn_GDA.Where(x => x.IPAddress == ip);
                foreach (var user in userLog) {
                    if (!(user.ExpiresOn == null)) {
                        if (user.ExpiresOn > DateTime.Now) {
                            return user.User_ID;                        
                        }                                            
                    } 
                }
                return 0;    
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in CheckIfLoggedIn: " + e.Message);
                return 0;
            }  
        }  

        // For the UserLoggedIn_GDA table.
        public void SetExpiresOnValue(int userid, string ip) 
        {
            try
            {  
                try {
                    var userLog = db.UserLoggedIn_GDA.First(x => x.User_ID == userid && x.IPAddress == ip);
                    if (!(userLog == null)) {
                        userLog.ExpiresOn = DateTime.Now.AddDays(1);
                        db.SaveChanges(); 
                    }
                } catch {
                    UserLoggedIn_GDA newLogin = new UserLoggedIn_GDA
                    {
                        User_ID = userid,
                        ExpiresOn = DateTime.Now.AddDays(1),
                        IPAddress = ip
                    };
                    db.UserLoggedIn_GDA.Add(newLogin);
                    db.SaveChanges();
                }
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in SetExpiresOnValue: " + e.Message);
                var r = e;
            }  
        } 

        // For the UserLoggedIn_GDA table.  This is called when user logs out manually.
        public void SetExpiresToNull(int userid) 
        {
            try
            {  
                var userLog = db.UserLoggedIn_GDA.First(x => x.User_ID == userid);

                if (!(userLog == null)) {
                    userLog.ExpiresOn = DateTime.Now.AddDays(1); // should be null?
                    db.SaveChanges(); 
                }         
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in SetExpiresToNull: " + e.Message);
            }  
        } 

        
        public int SignOut(int userID)  
        {  
            try  
            {  
                var results = db.UserLoggedIn_GDA.Where(x => x.User_ID == userID);
                foreach (var user in results)
                {
                    // This forces user to login next time.
                    user.ExpiresOn = null;
                }
                db.SaveChanges();
                return 1;                                   
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in SignOut: " + e.Message);
                return 0;
            }  
        }


        // First make a Conversation_GDA instance and get the Conversation_ID.  Then make a Message_GDA instance using the Conversation_ID.
        public int StartConversation(Conversation_GDA convo, string message) 
        {
            try
            {  
                var CreateConversationResult = CreateConversation(convo);
                         
                if (CreateConversationResult != 0) {
                    Message_GDA msg = new Message_GDA(CreateConversationResult, convo.Person1_ID, convo.Person2_ID, message)
                    {
                        DateCreated = DateTime.Now
                    };
                    db.Messages_GDA.Add(msg);
                    db.SaveChanges();
                    
                    return 1;
                }                
                else {
                    Debug.WriteLine("Unable to make conversation...");
                    return 0;
                }
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in StartConversation: " + e.Message);
                return 0;
            }  
        }

        public int CreateConversation(Conversation_GDA convo) 
        {
            try 
            {
                try {
                    var checkForAlreadyMadeConvo = db.Conversations_GDA.First(x => (x.Person1_ID == convo.Person1_ID 
                        && x.Person2_ID == convo.Person2_ID) || (x.Person1_ID == convo.Person2_ID 
                        && x.Person2_ID == convo.Person1_ID));
                    if (!(checkForAlreadyMadeConvo == null)) {
                        return checkForAlreadyMadeConvo.Conversation_ID;
                    }
                } catch (Exception e) {
                    Debug.WriteLine("Error checking for already made convo: " + e);
                }

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var conn = MyConfig.GetValue<string>("ConnectionStrings:AZURE_SQL_CONNECTION_2");

                var convoID = 0;
                using(SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();
                    string sql = "INSERT INTO Conversations_GDA (Person1_ID,Person2_ID,Person1_InConvo,Person2_InConvo) VALUES (@param1,@param2,@param3,@param4); SELECT scope_identity();";
                    using(SqlCommand cmd = new SqlCommand(sql,connection)) 
                    {
                        cmd.Parameters.Add("@param1", SqlDbType.Int).Value = convo.Person1_ID;  
                        cmd.Parameters.Add("@param2", SqlDbType.Int).Value = convo.Person2_ID;
                        cmd.Parameters.Add("@param3", SqlDbType.Bit).Value = convo.Person1_InConvo;
                        cmd.Parameters.Add("@param4", SqlDbType.Bit).Value = convo.Person2_InConvo;
                        cmd.CommandType = CommandType.Text;
                        convoID = Convert.ToInt32(cmd.ExecuteScalar()); 
                    }
                    connection.Close();
                }    
                return convoID; 
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in CreateConversation: " + e.Message);
                return 0;
            }             
        }
        
        public int SendMessageToAnotherUser(Message_GDA message) 
        {
            try
            {  
                db.Messages_GDA.Add(message);
                db.SaveChanges();
                return 1;                
            }  
            catch (Exception e)
            {
                Debug.WriteLine("Error in SendMessageToAnotherUser: " + e.Message);
                return 0;
            }  
        }      
    }  
}
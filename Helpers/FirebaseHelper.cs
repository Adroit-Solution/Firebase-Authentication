using Firebase.Authentication.Constants;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Firebase.Authentication.Helpers
{
    public class FirebaseHelper
    {
        private readonly FirebaseAuth firebaseAuth;
        private readonly FirebaseApp firebaseApp;
        private readonly FirebaseConfig _firebaseConfig;

        public FirebaseHelper(IOptions<FirebaseConfig> firebaseConfig)
        {
            firebaseApp = FirebaseApp.DefaultInstance;
            firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
            _firebaseConfig = firebaseConfig.Value;
        }

        public async Task<string> CreateUserAsync(string email, string password)
        {
            var userRecord = await firebaseAuth.CreateUserAsync(new UserRecordArgs
            {
                Email = email,
                Password = password
            });
            return userRecord.Uid;
        }

        public async Task<JsonDocument> SignInWithEmailAsync(string email, string password)
        {
            //var userRecord = await firebaseAuth.GetUserByEmailAsync(email);
            //if(userRecord is null)
            //    throw new Exception("User not found.");

            //var token = await firebaseAuth.CreateCustomTokenAsync(userRecord.Uid);
            //return token;

            using var client = new HttpClient();

            var content = new StringContent(JsonSerializer.Serialize(new
            {
                email = email,
                password = password,
                returnSecureToken = true
            }), Encoding.UTF8, "application/json");

            var result = await client.PostAsync(string.Format(FirebaseConstants.SignInWithEmailApi, _firebaseConfig.APIKey), content);
            if (!result.IsSuccessStatusCode)
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                throw new Exception($"Error signing in: {errorContent}");
            }

            var responseContent = await result.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            return responseJson;
        }

        public async Task<string> GetUserIdAsync(string token)
        {
            var decodedToken = await firebaseAuth.VerifyIdTokenAsync(token);
            return decodedToken.Uid;
        }

        public async Task DeleteUserAsync(string uid)
        {
            await firebaseAuth.DeleteUserAsync(uid);
        }

        public async Task UpdateUserAsync(string uid, string email = null, string password = null)
        {
            var updateArgs = new UserRecordArgs
            {
                Uid = uid
            };
            if (!string.IsNullOrEmpty(email))
            {
                updateArgs.Email = email;
            }
            if (!string.IsNullOrEmpty(password))
            {
                updateArgs.Password = password;
            }
            await firebaseAuth.UpdateUserAsync(updateArgs);
        }

        public async Task<UserRecord> GetUserAsync(string uid)
        {
            return await firebaseAuth.GetUserAsync(uid);
        }

        public async Task<JsonDocument> GenerateTokenFromRefreshTokenAsync(string refreshToken)
        {
            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                grant_type = "refresh_token",
                refresh_token = refreshToken,
            }), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(string.Format(FirebaseConstants.GenerateTokenFromRefreshToken, _firebaseConfig.APIKey), content);
            if (!result.IsSuccessStatusCode)
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                throw new Exception($"Error generating token: {errorContent}");
            }
            var responseContent = await result.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            return responseJson;
        }

    }
}

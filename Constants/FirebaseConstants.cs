namespace Firebase.Authentication.Constants
{
    public static class FirebaseConstants
    {
        public static readonly string SignInWithEmailApi = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={0}";
        public static readonly string GenerateTokenFromRefreshToken = "https://securetoken.googleapis.com/v1/token?key={0}";
    }
}

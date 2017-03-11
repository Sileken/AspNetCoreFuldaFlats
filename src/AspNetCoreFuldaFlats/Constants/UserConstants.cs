namespace AspNetCoreFuldaFlats.Constants
{
    public static class UserConstants
    {
        public static string DefaultUserProfilePicture = "/uploads/cupcake.png";
        public static string PasswordSalt = "fuldaflats#2016#";
        public static int MaxSignInAttempts = 10;
        public static int MinPasswordLength = 5;

        public static class UserTypes
        {
            public static int Normal = 1;
            public static int Landlord = 2;
        }
    }
}

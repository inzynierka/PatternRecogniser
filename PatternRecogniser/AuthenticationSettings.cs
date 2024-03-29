﻿namespace PatternRecogniser
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public int JwtExpireTimeInMinutes { get; set; }
        public int RefreshTokenExpireTimeInMinutes { get; set; }
        public string JwtIssuer { get; set; }
    }
}

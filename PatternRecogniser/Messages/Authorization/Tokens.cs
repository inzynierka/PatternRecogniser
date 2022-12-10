namespace PatternRecogniser.Messages.Authorization
{
    // zaproponuj napisanie login w respond w rozdziale komunikacji
    // lub usunięcie login jako klucza i ustawienie jako klucza loginu
    // czmu o tym wcześniej nie pomyślałem nie wiem xD
    public class Tokens
    {
        public string accessToken {get; set;} 
        public string refreshToken {get; set;}
    }
}

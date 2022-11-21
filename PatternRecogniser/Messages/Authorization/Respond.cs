namespace PatternRecogniser.Messages.Authorization
{
    // zaproponuj napisanie userId w respond w rozdziale komunikacji
    // lub usunięcie userId jako klucza i ustawienie jako klucza loginu
    // czmu o tym wcześniej nie pomyślałem nie wiem xD
    public class Respond
    {
        public int userId { get; set; }
        public string accessToken {get; set;} 
    }
}

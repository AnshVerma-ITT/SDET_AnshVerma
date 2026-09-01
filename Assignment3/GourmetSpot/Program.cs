using GourmetSpot.UserInterface;
namespace GourmetSpot
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RestaurantApp restaurantApp = new RestaurantApp();
                restaurantApp.Run();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Application stopped because of an input/output error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected application error: {ex.Message}");
            }
        }
    }
}

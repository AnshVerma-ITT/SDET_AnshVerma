using GourmetSpot.Exceptions;
using GourmetSpot.UserInterface;
using GourmetSpot.Utilities;

namespace GourmetSpot
{
    public class GourmetSpotApp
    {
        public static void RunApplication()
        {
            try
            {
                RestaurantAppScreen restaurantAppScreen = new RestaurantAppScreen();
                restaurantAppScreen.Display();
            }
            catch (GourmetSpotException exception)
            {
                ExceptionUtilities.ShowError(ExceptionUtilities.GetMessage(exception));
            }
            catch (Exception exception)
            {
                ExceptionUtilities.ShowError("Unexpected application error: " + exception.Message);
            }
        }
    }
}

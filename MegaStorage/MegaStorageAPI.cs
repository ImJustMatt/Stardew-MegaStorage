using MegaStorage.API;

namespace MegaStorage
{
    public class MegaStorageAPI : IMegaStorageApi
    {
        public static IMegaStorageApi Instance { get; set; }

        /*********
        ** Events
        *********/


        /*********
        ** Public methods
        *********/
        public MegaStorageAPI()
        {
            Instance = this;
        }

        /*********
        ** Private methods
        *********/

    }
}

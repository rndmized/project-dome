
namespace EnumsServer
{
	/*
	 * Enums that are used as in ID for the packages so both the serves and the clients 
	 * knows what kind of data is in the package and handles it accodingly
	 */
	class Enums
	{
		public enum AllEnums
		{
			SSendingPlayerID = 1,
			SSendingAlreadyConnectedToMain = 2,
			SSendingMainToAlreadyConnected = 3,
			SSyncingPlayerMovement = 4,
			SSendingMessage = 5,
			SCloseConnection = 6,

			HListPlayers = 10,
			HKickPlayer = 11,
			HGetSettings = 12,
			HChangeSettings = 13,
			HRestartServer = 14
		}
	}
}

namespace DPMGallery.DBMigration
{
	public static class FieldLengths
	{
		public static int FieldLengthShort = 128;
		public static int FieldLengthMedium = 256;
		public static int FieldLengthLong = 1024;
		public static int FieldLengthVeryLong = 10240;
		public static int FieldLengthMax = 10485760; // Postgres maximum varying character length 

	}
}

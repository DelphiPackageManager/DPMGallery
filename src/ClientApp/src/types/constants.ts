export const SITE_URL = "https://delphi.dev";

export const Constants =
{
	DefaultPageSize: 5,
	FieldLength:
	{
		Medium: 256,
		Long: 1_024,
		VeryLong: 10_240,
		Max: 1_000_000_000 // SQLite default maximum string length (PostgreSQL is 10485760); 
	},

	Roles:
	{
		Administrator: "Administrator",
		RegisteredUser: "RegisteredUser"
	},

	RegExPatterns:
	{
		UserName: /^(?:\w[\w\-\.\+\@]+)$/,
	}


}
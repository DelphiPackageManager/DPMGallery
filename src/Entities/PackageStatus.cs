namespace DPMGallery.Entities
{
    public enum PackageStatus
    {
        Queued = 0, //when first created.
        ProcessingAV = 1,
        CopyToFileSystem = 2,
        FailedAV = 3,
        //leaving room for other statuses here!
        Passed = 200,
    }
}

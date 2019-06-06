namespace VERPS.WebApp.Models.General
{
    public enum StateMessage
    {
        None = 0,

        FailedUploadPhoto,
        FailedUploadPhotoExtensionNotCorrect,
        FailedUploadPhotoEmptyFile,
        FailedNotAllowedEditOwnAccount,
        FailedUserNotFound,
        FailedUsernameExists,
        FailedSave,
        FailedMissingInfo,
        FailedNotCorrectFilledIn,
        FailedResetPassword,
        FailedSendingOrder,
        FailedNoExactConnection,
        FailedOrderDoesNotExist,
        FailedPaymentConditionMissing,
        FailedOrderCreatorMissing,

        WarningNotCreatedYet,

        SuccessUploadPhoto,
        SuccessRemovedPhoto,
        SuccessSave,
        SuccessRemovedUser,
        SuccessResetPassword,
        SuccessSendingOrder,
        SuccessRemovedOrder,
        SuccessAddedItems,
    }
}

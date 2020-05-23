namespace MyEvernote.Entities.Messages
{
    //Hata mesajlarını bir koda döküyoruz. Bu şekilde kodların yönetimi kolaylaşacaktır.
    public enum ErrorMessageCode
    {
        UsernameAlreadyExists = 101,
        EmailAlreadyExists = 102,
        UserIsNotActive = 151,
        UsernameorPassWrong = 152,
        CheckYourEmail = 153,
        UserAlreadyActivate = 154,
        ActivateIdDoesNotExists = 155,
        UserNotFound = 156,
        ProfileCouldNotUpdated=157,
        UserCouldNotFind = 158,
        UserCouldNotRemove = 159,
        UserCouldNotInserted = 160,
        UserCouldNotUpdated = 161
    }
}

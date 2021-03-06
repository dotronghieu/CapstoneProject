﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.Helper
{
    public class Constants
    {
        public struct Const
        {
            public const int ROLE_ADMIN_ID = 1;
            public const int ROLE_USER_ID = 2;
            public const int NUMBER_OF_PHOTO_HOMEPAGE = 21;
            public const int NUMBER_OF_NOT_APPROVED_PHOTO = 30;
            public const string ROLE_ADMIN = "Admin";
            public const string ROLE_USER = "User";
            public const string IMAGO_EMAIL = "imagoworkplace@gmail.com";
            public const string IMAGO_EMAIL_PASSWORD = "sa@123456";
            public const string PHOTO_STATUS_PENDING = "pending";
            public const string PHOTO_STATUS_APPROVED = "approved";
            public const string PHOTO_STATUS_DENIED = "denied";
            public const string NOTIFICATION_1 = "posted a new photo";
            public const string NOTIFICATION_2 = "sold a exclusive photo";
        }
    }
}

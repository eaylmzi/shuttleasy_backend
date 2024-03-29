﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Resource.String
{
    public static class Error
    {
        public const string NOTCALCULATED = "The route is not calculated";
        public const string AlreadyFound = "The item is already in table";
        public const string AlreadyFinish = "This session is already finished";
        public const string AlreadyComment = "This user is already comment to this shuttle";
        public const string EmptyList = "The list is empty";
        public const string ForeignRequest = "The user and the person who sent the request are not the same";
        public const string FoundEmailOrTelephone = "Registered with this email or phone";
        public const string NotAdded = "Not Added";
        public const string NotCreatedUser = "Registered with this email or phone";
        public const string NotCorrectEmailAndPassword = "Email and password not correct";
        public const string NotDeletedPassenger = "The passenger not deleted";
        public const string NotDeletedPickupArea = "The pick-up area not deleted";
        public const string NotFoundAdmin = "The admin not found in list";
        public const string NotFoundCompany = "Not found company";
        public const string NotFoundShuttleSession = "Not Found Shuttle Session";
        public const string NotFoundDriver = "The driver not found in list";
        public const string NotFoundPassenger = "The passenger not found in list";
        public const string NotFoundSuperAdmin = "The Superadmin not found in list";
        public const string NotFound = "Not Found";
        public const string NotFoundUser= "The user that send request not found";
        public const string NotMatchedToken = "Mistake about token";
        public const string NotUpdatedInformation = "Not updated information";
        public const string NotVerifiedPassword = "The password not verified";
        public const string NotMatchedForeignKeys = "Foreign keys are not matched";
    }
}

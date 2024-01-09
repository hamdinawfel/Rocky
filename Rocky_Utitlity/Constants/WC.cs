﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rocky_Utility.Constants
{
    public static class WC
    {
        public const string ProductImagePath = @"\images\product\";
        public const string SessionCard = "ShoppingCradSession";
        public const string SessionInquiryId = "InquirySession";
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";


        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
            new List<string>
            {
                StatusApproved,StatusCancelled,StatusInProcess,StatusPending,StatusRefunded,StatusShipped
            });
    }

}
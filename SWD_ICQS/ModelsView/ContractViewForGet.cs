﻿namespace SWD_ICQS.ModelsView
{
    public class ContractViewForGet
    {
        public int AppointmentId { get; set; }
        public string? CustomerName { get; set; }
        public string? ContractorName { get; set; }
        public string? ContractUrl { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? EditDate { get; set; }
        public int? Status { get; set; }
        public string? Progress { get; set; }

    }
}
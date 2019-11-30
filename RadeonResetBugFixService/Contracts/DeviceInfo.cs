namespace RadeonResetBugFixService.Contracts
{
    using System;

    class DeviceInfo
    {
        public Guid ClassGuid { get; set; }

        public string ClassName { get; set; }

        public string Description => $"{this.Name} ({this.DeviceId}, {this.ClassGuid})";

        public string DeviceId { get; set; }

        public long? ErrorCode { get; set; }

        public bool IsDisabled => this.ErrorCode == 22;

        public bool IsPresent { get; set; }

        public string Manufacturer { get; set; }

        public string Name { get; set; }

        public string Service { get; set; }
    }
}

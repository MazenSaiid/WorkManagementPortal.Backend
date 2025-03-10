﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class ScreenShotTrackingLog
    {
        public int Id { get; set; }
        public DateTime ScreenShotTime { get; set; }
        public byte[] Screenshot { get; set; }
        public bool IsIdle { get; set; }
        public string SerializedTrackingObject { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int? WorkTrackingLogId { get; set; }
        public WorkTrackingLog WorkTrackingLog { get; set; }

    }
}

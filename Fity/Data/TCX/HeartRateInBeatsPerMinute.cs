﻿using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    [XmlType("HeartRateInBeatsPerMinute_t")]
    public class HeartRateInBeatsPerMinute
    {
        [XmlElement("Value")]
        public float Value { get; set; }
    }
}
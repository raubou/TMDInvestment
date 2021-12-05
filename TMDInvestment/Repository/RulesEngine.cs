using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Repository
{
    public class RulesEngine
    {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int UserId { get; set; }
		public bool UseDayTrade { get; set; }
		public bool UseSwingTrade { get; set; }
		public int? HistoryDayRange { get; set; }
		public int? HistoryIntervalRange { get; set; }
		public double? PercentBalanceUse { get; set; }
		public double? PriceRangeMax { get; set; }
		public double? PriceRangeMin { get; set; }
		public double? PercentRangeHigh { get; set; }
		public double? PercentRangeLow { get; set; }
		public double? SqueezeRangeHigh { get; set; }
		public double? SqueezeRangeLow { get; set; }
		public long? VolumeRangeHigh { get; set; }
		public long? VolumeRangLow { get; set; }
		public int? MaxHoldforStock { get; set; }
		public int? MaxRoundTradesPerDay { get; set; }
	[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime DateCreated { get; set; }
		public DateTime? DateUpdated { get; set; }
    }
}

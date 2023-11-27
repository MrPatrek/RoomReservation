﻿namespace Entities.DataTransferObjects
{
    public class RoomAvailableDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public decimal PriceTotal { get; set; }
        public string? DescriptionShort { get; set; }
        public string? DescriptionLong { get; set; }

        public IEnumerable<ImageDto>? Images { get; set; }
    }
}

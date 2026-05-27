using System;
using System.Collections.Generic;

namespace TableCharm.DTOs
{
    /// <summary>
    /// DTO for Distributor read operations
    /// </summary>
    public class DistributorDto
    {
        public int DistributorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int? ParentDistributorId { get; set; }
        public string ParentDistributorName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }

    /// <summary>
    /// DTO for creating a new distributor
    /// </summary>
    public class CreateDistributorDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int? ParentDistributorId { get; set; }
    }

    /// <summary>
    /// DTO for updating a distributor
    /// </summary>
    public class UpdateDistributorDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for distributor hierarchy with downline
    /// </summary>
    public class DistributorHierarchyDto
    {
        public int DistributorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public List<DistributorHierarchyDto> DirectDownline { get; set; } = new List<DistributorHierarchyDto>();
    }
}

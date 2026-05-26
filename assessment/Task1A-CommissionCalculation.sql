-- TASK 1A: SQL QUERY - Commission Calculation Engine
-- Purpose: Aggregate sales data for distributors across 3 levels (self, direct downline, second-level downline)
-- Returns: DistributorId, DistributorName, Level, and TotalSales for the specified date range

SET NOCOUNT ON;

-- Main Query: Returns sales aggregation for specified distributor and date range
SELECT 
    d.DistributorId,
    d.Name AS DistributorName,
    CASE 
        WHEN d.DistributorId = @DistributorId THEN 0  -- Self
        WHEN dParent.DistributorId = @DistributorId THEN 1  -- Direct downline
        WHEN dGrandParent.DistributorId = @DistributorId THEN 2  -- Second-level downline
    END AS Level,
    COALESCE(SUM(s.Amount), 0) AS TotalSales
FROM Distributors d
LEFT JOIN Distributors dParent ON d.ParentDistributorId = dParent.DistributorId
LEFT JOIN Distributors dGrandParent ON dParent.ParentDistributorId = dGrandParent.DistributorId
LEFT JOIN Sales s ON d.DistributorId = s.DistributorId 
    AND s.SaleDate >= @StartDate 
    AND s.SaleDate <= @EndDate
WHERE 
    -- Include the specified distributor (Level 0)
    d.DistributorId = @DistributorId
    -- OR include their direct downline (Level 1)
    OR d.ParentDistributorId = @DistributorId
    -- OR include second-level downline (Level 2)
    OR dParent.DistributorId = @DistributorId
GROUP BY 
    d.DistributorId,
    d.Name,
    CASE 
        WHEN d.DistributorId = @DistributorId THEN 0
        WHEN dParent.DistributorId = @DistributorId THEN 1
        WHEN dGrandParent.DistributorId = @DistributorId THEN 2
    END
ORDER BY 
    Level ASC,
    TotalSales DESC;

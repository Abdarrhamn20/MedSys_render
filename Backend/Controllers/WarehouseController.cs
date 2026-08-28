using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Models;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    [Authorize(Roles = "Admin,WarehouseKeeper")]
    public class WarehouseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  المخازن
        // ============================================================

        // GET: api/warehouse
        [HttpGet]
        public async Task<IActionResult> GetWarehouses()
        {
            var warehouses = await _context.Warehouses
                .OrderBy(w => w.WarehouseCode)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(warehouses));
        }

        // POST: api/warehouse
        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] WarehouseDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.WarehouseNameAr) || string.IsNullOrWhiteSpace(dto.WarehouseCode))
                return BadRequest(ApiResponse.Fail("اسم المخزن بالعربية والكود مطلوبان"));

            var codeExists = await _context.Warehouses.AnyAsync(w => w.WarehouseCode == dto.WarehouseCode);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("كود المخزن مستخدم مسبقاً"));

            var warehouse = new Warehouse
            {
                WarehouseName = string.IsNullOrWhiteSpace(dto.WarehouseName) ? dto.WarehouseNameAr.Trim() : dto.WarehouseName.Trim(),
                WarehouseNameAr = dto.WarehouseNameAr.Trim(),
                WarehouseCode = dto.WarehouseCode.Trim(),
                Location = dto.Location,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Warehouses.Add(warehouse);

            await AuditAsync("WarehouseCreated", "Warehouse", warehouse.WarehouseID, $"إنشاء مخزن {warehouse.WarehouseNameAr} بكود {warehouse.WarehouseCode}");

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { warehouse.WarehouseID }, "تم إنشاء المخزن بنجاح"));
        }

        // PUT: api/warehouse/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDTO dto)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound(ApiResponse.Fail("المخزن غير موجود"));

            var codeExists = await _context.Warehouses.AnyAsync(w => w.WarehouseCode == dto.WarehouseCode && w.WarehouseID != id);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("كود المخزن مستخدم مسبقاً"));

            warehouse.WarehouseName = string.IsNullOrWhiteSpace(dto.WarehouseName) ? dto.WarehouseNameAr.Trim() : dto.WarehouseName.Trim();
            warehouse.WarehouseNameAr = dto.WarehouseNameAr.Trim();
            warehouse.WarehouseCode = dto.WarehouseCode.Trim();
            warehouse.Location = dto.Location;
            warehouse.IsActive = dto.IsActive;

            await AuditAsync("WarehouseUpdated", "Warehouse", warehouse.WarehouseID, $"تعديل مخزن {warehouse.WarehouseNameAr}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث المخزن بنجاح"));
        }

        // DELETE: api/warehouse/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound(ApiResponse.Fail("المخزن غير موجود"));

            var hasMovements = await _context.StockMovements.AnyAsync(m => m.WarehouseID == id || m.ToWarehouseID == id);
            if (hasMovements)
                return BadRequest(ApiResponse.Fail("لا يمكن حذف مخزن مرتبط بسندات. عطّله بدلاً من الحذف."));

            _context.Warehouses.Remove(warehouse);

            await AuditAsync("WarehouseDeleted", "Warehouse", id, $"حذف مخزن {warehouse.WarehouseNameAr}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف المخزن بنجاح"));
        }

        // ============================================================
        //  فئات الأصناف (شجرة)
        // ============================================================

        // GET: api/warehouse/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.InventoryCategories
                .OrderBy(c => c.CategoryNameAr)
                .ToListAsync();

            var tree = categories
                .Where(c => c.ParentCategoryID == null)
                .Select(c => BuildCategoryNode(c, categories))
                .ToList();

            return Ok(ApiResponse<object>.Ok(tree));
        }

        // GET: api/warehouse/categories/flat
        [HttpGet("categories/flat")]
        public async Task<IActionResult> GetCategoriesFlat()
        {
            var categories = await _context.InventoryCategories
                .OrderBy(c => c.CategoryNameAr)
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.CategoryNameAr,
                    c.ParentCategoryID,
                    c.IsActive
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(categories));
        }

        // POST: api/warehouse/categories
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] InventoryCategoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CategoryNameAr))
                return BadRequest(ApiResponse.Fail("اسم الفئة بالعربية مطلوب"));

            if (dto.ParentCategoryID.HasValue)
            {
                var parent = await _context.InventoryCategories.FindAsync(dto.ParentCategoryID.Value);
                if (parent == null)
                    return BadRequest(ApiResponse.Fail("الفئة الأب غير موجودة"));
            }

            var category = new InventoryCategory
            {
                CategoryName = string.IsNullOrWhiteSpace(dto.CategoryName) ? dto.CategoryNameAr.Trim() : dto.CategoryName.Trim(),
                CategoryNameAr = dto.CategoryNameAr.Trim(),
                ParentCategoryID = dto.ParentCategoryID,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.InventoryCategories.Add(category);

            await AuditAsync("CategoryCreated", "InventoryCategory", category.CategoryID, $"إنشاء فئة أصناف {category.CategoryNameAr}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { category.CategoryID }, "تم إنشاء الفئة بنجاح"));
        }

        // PUT: api/warehouse/categories/5
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] InventoryCategoryDTO dto)
        {
            var category = await _context.InventoryCategories.FindAsync(id);
            if (category == null)
                return NotFound(ApiResponse.Fail("الفئة غير موجودة"));

            if (dto.ParentCategoryID.HasValue)
            {
                if (dto.ParentCategoryID.Value == id)
                    return BadRequest(ApiResponse.Fail("لا يمكن أن تكون الفئة أباً لنفسها"));
                var parent = await _context.InventoryCategories.FindAsync(dto.ParentCategoryID.Value);
                if (parent == null)
                    return BadRequest(ApiResponse.Fail("الفئة الأب غير موجودة"));
            }

            category.CategoryName = string.IsNullOrWhiteSpace(dto.CategoryName) ? dto.CategoryNameAr.Trim() : dto.CategoryName.Trim();
            category.CategoryNameAr = dto.CategoryNameAr.Trim();
            category.ParentCategoryID = dto.ParentCategoryID;
            category.IsActive = dto.IsActive;

            await AuditAsync("CategoryUpdated", "InventoryCategory", category.CategoryID, $"تعديل فئة {category.CategoryNameAr}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث الفئة بنجاح"));
        }

        // ============================================================
        //  الأصناف
        // ============================================================

        // GET: api/warehouse/items?categoryId=...&search=...&page=...
        [HttpGet("items")]
        public async Task<IActionResult> GetItems(
            [FromQuery] int? categoryId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.InventoryItems.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryID == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.ItemNameAr.Contains(search) || i.ItemName.Contains(search) || i.ItemCode.Contains(search));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(i => i.ItemCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new
                {
                    i.ItemID,
                    i.ItemCode,
                    i.ItemName,
                    i.ItemNameAr,
                    i.CategoryID,
                    CategoryNameAr = i.Category.CategoryNameAr,
                    i.Unit,
                    i.PurchasePrice,
                    i.SellingPrice,
                    i.ReorderLevel,
                    i.Manufacturer,
                    i.ExpiryDate,
                    i.IsActive,
                    i.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = items.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/warehouse/items/5
        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _context.InventoryItems
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.ItemID == id);

            if (item == null)
                return NotFound(ApiResponse.Fail("الصنف غير موجود"));

            return Ok(ApiResponse<object>.Ok(item));
        }

        // POST: api/warehouse/items
        [HttpPost("items")]
        public async Task<IActionResult> CreateItem([FromBody] InventoryItemDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ItemCode) || string.IsNullOrWhiteSpace(dto.ItemNameAr))
                return BadRequest(ApiResponse.Fail("كود الصنف واسمه بالعربية مطلوبان"));

            var codeExists = await _context.InventoryItems.AnyAsync(i => i.ItemCode == dto.ItemCode);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("كود الصنف مستخدم مسبقاً"));

            var category = await _context.InventoryCategories.FindAsync(dto.CategoryID);
            if (category == null)
                return BadRequest(ApiResponse.Fail("الفئة المحددة غير موجودة"));

            var item = new InventoryItem
            {
                ItemCode = dto.ItemCode.Trim(),
                ItemName = string.IsNullOrWhiteSpace(dto.ItemName) ? dto.ItemNameAr.Trim() : dto.ItemName.Trim(),
                ItemNameAr = dto.ItemNameAr.Trim(),
                CategoryID = dto.CategoryID,
                MedicationID = dto.MedicationID,
                Unit = dto.Unit,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                ReorderLevel = dto.ReorderLevel,
                Manufacturer = dto.Manufacturer,
                ExpiryDate = dto.ExpiryDate,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.InventoryItems.Add(item);

            await AuditAsync("ItemCreated", "InventoryItem", item.ItemID, $"إنشاء صنف {item.ItemNameAr} بكود {item.ItemCode}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { item.ItemID }, "تم إنشاء الصنف بنجاح"));
        }

        // PUT: api/warehouse/items/5
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] InventoryItemDTO dto)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound(ApiResponse.Fail("الصنف غير موجود"));

            var codeExists = await _context.InventoryItems.AnyAsync(i => i.ItemCode == dto.ItemCode && i.ItemID != id);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("كود الصنف مستخدم مسبقاً"));

            var category = await _context.InventoryCategories.FindAsync(dto.CategoryID);
            if (category == null)
                return BadRequest(ApiResponse.Fail("الفئة المحددة غير موجودة"));

            item.ItemCode = dto.ItemCode.Trim();
            item.ItemName = string.IsNullOrWhiteSpace(dto.ItemName) ? dto.ItemNameAr.Trim() : dto.ItemName.Trim();
            item.ItemNameAr = dto.ItemNameAr.Trim();
            item.CategoryID = dto.CategoryID;
            item.MedicationID = dto.MedicationID;
            item.Unit = dto.Unit;
            item.PurchasePrice = dto.PurchasePrice;
            item.SellingPrice = dto.SellingPrice;
            item.ReorderLevel = dto.ReorderLevel;
            item.Manufacturer = dto.Manufacturer;
            item.ExpiryDate = dto.ExpiryDate;
            item.IsActive = dto.IsActive;

            await AuditAsync("ItemUpdated", "InventoryItem", item.ItemID, $"تعديل صنف {item.ItemNameAr}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث الصنف بنجاح"));
        }

        // ============================================================
        //  سندات المخزن (إدخال / إخراج / تحويل)
        // ============================================================

        // GET: api/warehouse/movements?type=...&status=...&from=...&to=...&page=...
        [HttpGet("movements")]
        public async Task<IActionResult> GetMovements(
            [FromQuery] string? type,
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.StockMovements.AsQueryable();

            if (!string.IsNullOrEmpty(type))
                query = query.Where(m => m.MovementType == type);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status == status);

            if (from.HasValue)
                query = query.Where(m => m.MovementDate >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(m => m.MovementDate < to.Value.Date.AddDays(1));

            var totalCount = await query.CountAsync();

            var movements = await query
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.MovementID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MovementID,
                    m.MovementNumber,
                    m.MovementType,
                    m.MovementDate,
                    m.WarehouseID,
                    WarehouseNameAr = m.Warehouse.WarehouseNameAr,
                    ToWarehouseNameAr = m.ToWarehouse != null ? m.ToWarehouse.WarehouseNameAr : null,
                    m.ReferenceType,
                    m.Notes,
                    m.Status,
                    m.CreatedAt,
                    m.PostedAt,
                    CreatedByName = m.CreatedByUser != null ? m.CreatedByUser.FullName : null,
                    ItemsCount = m.Items.Count,
                    TotalQuantity = m.Items.Sum(i => i.Quantity),
                    TotalValue = m.Items.Sum(i => i.Quantity * i.UnitPrice)
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = movements.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/warehouse/movements/5
        [HttpGet("movements/{id}")]
        public async Task<IActionResult> GetMovement(int id)
        {
            var movement = await _context.StockMovements
                .Include(m => m.Warehouse)
                .Include(m => m.ToWarehouse)
                .Include(m => m.CreatedByUser)
                .Include(m => m.PostedByUser)
                .Include(m => m.Items)
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(m => m.MovementID == id);

            if (movement == null)
                return NotFound(ApiResponse.Fail("السند غير موجود"));

            var result = new
            {
                movement.MovementID,
                movement.MovementNumber,
                movement.MovementType,
                movement.MovementDate,
                movement.WarehouseID,
                WarehouseNameAr = movement.Warehouse.WarehouseNameAr,
                movement.ToWarehouseID,
                ToWarehouseNameAr = movement.ToWarehouse?.WarehouseNameAr,
                movement.ReferenceType,
                movement.ReferenceID,
                movement.Notes,
                movement.Status,
                movement.CreatedAt,
                movement.PostedAt,
                CreatedByName = movement.CreatedByUser?.FullName,
                PostedByName = movement.PostedByUser?.FullName,
                Items = movement.Items
                    .Select(i => new
                    {
                        i.StockMovementItemID,
                        i.ItemID,
                        ItemCode = i.Item.ItemCode,
                        ItemNameAr = i.Item.ItemNameAr,
                        i.Quantity,
                        i.UnitPrice,
                        i.TotalPrice,
                        i.Notes
                    })
                    .ToList()
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/warehouse/movements
        [HttpPost("movements")]
        public async Task<IActionResult> CreateMovement([FromBody] StockMovementDTO dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(ApiResponse.Fail("يجب إدخال صنف واحد على الأقل في السند"));

            var validTypes = new[] { "In", "Out", "Transfer" };
            if (!validTypes.Contains(dto.MovementType))
                return BadRequest(ApiResponse.Fail("نوع السند غير صالح (In/Out/Transfer)"));

            var warehouse = await _context.Warehouses.FindAsync(dto.WarehouseID);
            if (warehouse == null || !warehouse.IsActive)
                return BadRequest(ApiResponse.Fail("المخزن غير موجود أو غير مفعّل"));

            if (dto.MovementType == "Transfer")
            {
                if (!dto.ToWarehouseID.HasValue || dto.ToWarehouseID.Value == dto.WarehouseID)
                    return BadRequest(ApiResponse.Fail("حدد مخزن تحويل مختلف عن مخزن المصدر"));
                var toWarehouse = await _context.Warehouses.FindAsync(dto.ToWarehouseID.Value);
                if (toWarehouse == null || !toWarehouse.IsActive)
                    return BadRequest(ApiResponse.Fail("مخزن التحويل إليه غير موجود أو غير مفعّل"));
            }

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var movement = new StockMovement
            {
                MovementNumber = await GenerateMovementNumberAsync(),
                MovementType = dto.MovementType,
                MovementDate = dto.MovementDate == default ? DateTime.Now : dto.MovementDate,
                WarehouseID = dto.WarehouseID,
                ToWarehouseID = dto.MovementType == "Transfer" ? dto.ToWarehouseID : null,
                ReferenceType = string.IsNullOrWhiteSpace(dto.ReferenceType) ? "Adjustment" : dto.ReferenceType,
                ReferenceID = dto.ReferenceID,
                Notes = dto.Notes?.Trim() ?? string.Empty,
                Status = "Draft",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now
            };

            foreach (var line in dto.Items)
            {
                if (line.ItemID <= 0 || line.Quantity <= 0)
                    return BadRequest(ApiResponse.Fail("رقم صنف أو كمية غير صالحة في سند المخزن"));

                var item = await _context.InventoryItems.FindAsync(line.ItemID);
                if (item == null || !item.IsActive)
                    return BadRequest(ApiResponse.Fail("أحد الأصناف غير موجود أو غير مفعّل"));

                movement.Items.Add(new StockMovementItem
                {
                    ItemID = line.ItemID,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    Notes = line.Notes
                });
            }

            _context.StockMovements.Add(movement);

            await AuditAsync("MovementCreated", "StockMovement", movement.MovementID, $"إنشاء سند مخزن {movement.MovementNumber} ({movement.MovementType})");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { movement.MovementID, movement.MovementNumber }, "تم إنشاء السند بنجاح (بالحالة مسودة)"));
        }

        // POST: api/warehouse/movements/5/post
        [HttpPost("movements/{id}/post")]
        public async Task<IActionResult> PostMovement(int id)
        {
            var movement = await _context.StockMovements
                .Include(m => m.Items)
                .FirstOrDefaultAsync(m => m.MovementID == id);

            if (movement == null)
                return NotFound(ApiResponse.Fail("السند غير موجود"));

            if (movement.Status == "Posted")
                return BadRequest(ApiResponse.Fail("السند مرحّل بالفعل"));

            if (movement.Status == "Reversed")
                return BadRequest(ApiResponse.Fail("لا يمكن ترحيل سند معكوس"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            // التحقق من توفر الكميات للإخراج أو التحويل
            if (movement.MovementType == "Out")
            {
                foreach (var line in movement.Items)
                {
                    var available = await GetStockAsync(line.ItemID, movement.WarehouseID);
                    if (line.Quantity > available)
                        return BadRequest(ApiResponse.Fail($"الكمية غير كافية للصنف (مخزون {available:N2}) لسند الإخراج"));
                }
            }
            else if (movement.MovementType == "Transfer")
            {
                if (!movement.ToWarehouseID.HasValue)
                    return BadRequest(ApiResponse.Fail("مخزن التحويل إليه غير محدد"));

                foreach (var line in movement.Items)
                {
                    var available = await GetStockAsync(line.ItemID, movement.WarehouseID);
                    if (line.Quantity > available)
                        return BadRequest(ApiResponse.Fail($"الكمية غير كافية للصنف (مخزون {available:N2}) لسند التحويل"));
                }
            }

            movement.Status = "Posted";
            movement.PostedAt = DateTime.Now;
            movement.PostedByUserID = userId;

            await AuditAsync("MovementPosted", "StockMovement", movement.MovementID, $"ترحيل سند مخزن {movement.MovementNumber} ({movement.MovementType})");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم ترحيل سند المخزن {movement.MovementNumber} بنجاح"));
        }

        // POST: api/warehouse/movements/5/reverse
        [HttpPost("movements/{id}/reverse")]
        public async Task<IActionResult> ReverseMovement(int id)
        {
            var movement = await _context.StockMovements
                .Include(m => m.Items)
                .FirstOrDefaultAsync(m => m.MovementID == id);

            if (movement == null)
                return NotFound(ApiResponse.Fail("السند غير موجود"));

            if (movement.Status != "Posted")
                return BadRequest(ApiResponse.Fail("يُعكس فقط السند المرحّل"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            // عند عكس سند إدخال/تحويل يجب ألا يصبح الرصيد سالباً
            if (movement.MovementType == "In")
            {
                foreach (var line in movement.Items)
                {
                    var current = await GetStockAsync(line.ItemID, movement.WarehouseID);
                    if (line.Quantity > current)
                        return BadRequest(ApiResponse.Fail($"لا يمكن عكس سند الإدخال: رصيد الصنف غير كافٍ في المخزن ({current:N2})"));
                }
            }
            else if (movement.MovementType == "Transfer")
            {
                if (!movement.ToWarehouseID.HasValue)
                    return BadRequest(ApiResponse.Fail("مخزن التحويل إليه غير محدد"));
                foreach (var line in movement.Items)
                {
                    var current = await GetStockAsync(line.ItemID, movement.ToWarehouseID.Value);
                    if (line.Quantity > current)
                        return BadRequest(ApiResponse.Fail($"لا يمكن عكس سند التحويل: رصيد الصنف غير كافٍ في مخزن الوجهة ({current:N2})"));
                }
            }

            movement.Status = "Reversed";

            await AuditAsync("MovementReversed", "StockMovement", movement.MovementID, $"عكس سند مخزن {movement.MovementNumber} ({movement.MovementType})");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم عكس سند المخزن {movement.MovementNumber} بنجاح"));
        }

        // ============================================================
        //  الكميات والحركة
        // ============================================================

        // GET: api/warehouse/stock?warehouseId=...
        [HttpGet("stock")]
        public async Task<IActionResult> GetStock([FromQuery] int? warehouseId, [FromQuery] int? categoryId)
        {
            var result = await ComputeStockRowsAsync();

            var rows = result
                .Where(r => !warehouseId.HasValue || r.WarehouseID == warehouseId.Value)
                .Where(r => !categoryId.HasValue || r.CategoryID == categoryId.Value)
                .OrderBy(r => r.ItemCode)
                .Cast<object>()
                .ToList();

            return Ok(ApiResponse<object>.Ok(rows));
        }

        private async Task<List<StockRow>> ComputeStockRowsAsync()
        {
            var postedLines = await _context.StockMovementItems
                .Include(i => i.Movement)
                .Include(i => i.Item)
                    .ThenInclude(it => it.Category)
                .Where(i => i.Movement.Status == "Posted")
                .ToListAsync();

            var result = new List<StockRow>();

            foreach (var line in postedLines)
            {
                AddStockRow(result, line, line.Movement.WarehouseID);
                if (line.Movement.MovementType == "Transfer" && line.Movement.ToWarehouseID.HasValue)
                    AddStockRow(result, line, line.Movement.ToWarehouseID.Value);
            }

            return result;
        }

        private void AddStockRow(List<StockRow> list, StockMovementItem line, int warehouseId)
        {
            var sign = line.Movement.MovementType == "In" ? +1m
                     : line.Movement.MovementType == "Out" ? -1m
                     : line.Movement.WarehouseID == warehouseId ? -1m : +1m;

            var existing = list.FirstOrDefault(r => r.ItemID == line.ItemID && r.WarehouseID == warehouseId);

            if (existing != null)
            {
                existing.Quantity += sign * line.Quantity;
            }
            else
            {
                list.Add(new StockRow
                {
                    ItemID = line.ItemID,
                    ItemCode = line.Item.ItemCode,
                    ItemNameAr = line.Item.ItemNameAr,
                    CategoryID = line.Item.CategoryID,
                    CategoryNameAr = line.Item.Category.CategoryNameAr,
                    Unit = line.Item.Unit,
                    ReorderLevel = line.Item.ReorderLevel,
                    WarehouseID = warehouseId,
                    Quantity = sign * line.Quantity
                });
            }
        }

        // GET: api/warehouse/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await ComputeStockRowsAsync();

            var low = result
                .Where(r => r.Quantity <= r.ReorderLevel)
                .OrderBy(r => r.Quantity)
                .Cast<object>()
                .ToList();

            return Ok(ApiResponse<object>.Ok(low));
        }

        private class StockRow
        {
            public int ItemID { get; set; }
            public string ItemCode { get; set; } = string.Empty;
            public string ItemNameAr { get; set; } = string.Empty;
            public int CategoryID { get; set; }
            public string CategoryNameAr { get; set; } = string.Empty;
            public string Unit { get; set; } = string.Empty;
            public int ReorderLevel { get; set; }
            public int WarehouseID { get; set; }
            public decimal Quantity { get; set; }
        }

        // ============================================================
        //  الجرد الدوري (Stock Count)
        // ============================================================

        // GET: api/warehouse/counts?status=...&from=...&to=...&page=...
        [HttpGet("counts")]
        public async Task<IActionResult> GetCounts(
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.StockCounts.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(c => c.Status == status);

            if (from.HasValue)
                query = query.Where(c => c.CountDate >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(c => c.CountDate < to.Value.Date.AddDays(1));

            var totalCount = await query.CountAsync();

            var counts = await query
                .OrderByDescending(c => c.CountDate)
                .ThenByDescending(c => c.StockCountID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.StockCountID,
                    c.StockCountNumber,
                    c.CountDate,
                    c.WarehouseID,
                    WarehouseNameAr = c.Warehouse.WarehouseNameAr,
                    c.Status,
                    c.CreatedAt,
                    c.PostedAt,
                    c.Notes,
                    ItemsCount = c.Items.Count,
                    TotalSystem = c.Items.Sum(i => i.SystemQuantity),
                    TotalCounted = c.Items.Sum(i => i.CountedQuantity),
                    TotalDifference = c.Items.Sum(i => i.CountedQuantity - i.SystemQuantity),
                    CreatedByName = c.CreatedByUser != null ? c.CreatedByUser.FullName : null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = counts.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/warehouse/counts/5
        [HttpGet("counts/{id}")]
        public async Task<IActionResult> GetCount(int id)
        {
            var count = await _context.StockCounts
                .Include(c => c.Warehouse)
                .Include(c => c.CreatedByUser)
                .Include(c => c.PostedByUser)
                .Include(c => c.ReversedByUser)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(c => c.StockCountID == id);

            if (count == null)
                return NotFound(ApiResponse.Fail("سند الجرد غير موجود"));

            var result = new
            {
                count.StockCountID,
                count.StockCountNumber,
                count.CountDate,
                count.WarehouseID,
                WarehouseNameAr = count.Warehouse.WarehouseNameAr,
                count.Status,
                count.CreatedAt,
                count.PostedAt,
                count.ReversedAt,
                count.Notes,
                CreatedByName = count.CreatedByUser?.FullName,
                PostedByName = count.PostedByUser?.FullName,
                Items = count.Items
                    .Select(i => new
                    {
                        i.StockCountItemID,
                        i.ItemID,
                        ItemCode = i.Item.ItemCode,
                        ItemNameAr = i.Item.ItemNameAr,
                        Unit = i.Item.Unit,
                        i.SystemQuantity,
                        i.CountedQuantity,
                        Difference = i.CountedQuantity - i.SystemQuantity,
                        i.UnitPrice,
                        i.Notes
                    })
                    .OrderBy(i => i.ItemCode)
                    .ToList()
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/warehouse/counts
        [HttpPost("counts")]
        public async Task<IActionResult> CreateCount([FromBody] StockCountDTO dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(ApiResponse.Fail("يجب إدخال صنف واحد على الأقل في الجرد"));

            var warehouse = await _context.Warehouses.FindAsync(dto.WarehouseID);
            if (warehouse == null || !warehouse.IsActive)
                return BadRequest(ApiResponse.Fail("المخزن غير موجود أو غير مفعّل"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var count = new StockCount
            {
                StockCountNumber = await GenerateCountNumberAsync(),
                CountDate = dto.CountDate == default ? DateTime.Now : dto.CountDate,
                WarehouseID = dto.WarehouseID,
                Notes = dto.Notes?.Trim() ?? string.Empty,
                Status = "Draft",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now
            };

            foreach (var line in dto.Items)
            {
                if (line.ItemID <= 0 || line.CountedQuantity < 0)
                    return BadRequest(ApiResponse.Fail("رقم صنف أو كمية جرد غير صالحة"));

                var item = await _context.InventoryItems.FindAsync(line.ItemID);
                if (item == null || !item.IsActive)
                    return BadRequest(ApiResponse.Fail("أحد الأصناف غير موجود أو غير مفعّل"));

                var systemQty = await GetStockAsync(line.ItemID, dto.WarehouseID);

                count.Items.Add(new StockCountItem
                {
                    ItemID = line.ItemID,
                    SystemQuantity = systemQty,
                    CountedQuantity = line.CountedQuantity,
                    UnitPrice = item.PurchasePrice,
                    Notes = line.Notes
                });
            }

            if (count.Items.Select(i => i.ItemID).Distinct().Count() != count.Items.Count)
                return BadRequest(ApiResponse.Fail("لا يمكن تكرار نفس الصنف في سند الجرد"));

            _context.StockCounts.Add(count);

            await AuditAsync("StockCountCreated", "StockCount", count.StockCountID, $"إنشاء سند جرد {count.StockCountNumber}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { count.StockCountID, count.StockCountNumber }, "تم إنشاء سند الجرد بنجاح (بالحالة مسودة)"));
        }

        // PUT: api/warehouse/counts/5
        [HttpPut("counts/{id}")]
        public async Task<IActionResult> UpdateCount(int id, [FromBody] StockCountDTO dto)
        {
            var count = await _context.StockCounts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.StockCountID == id);

            if (count == null)
                return NotFound(ApiResponse.Fail("سند الجرد غير موجود"));

            if (count.Status != "Draft")
                return BadRequest(ApiResponse.Fail("يمكن تعديل سند الجرد في الحالة المسودة فقط"));

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(ApiResponse.Fail("يجب إدخال صنف واحد على الأقل في الجرد"));

            var warehouse = await _context.Warehouses.FindAsync(dto.WarehouseID);
            if (warehouse == null || !warehouse.IsActive)
                return BadRequest(ApiResponse.Fail("المخزن غير موجود أو غير مفعّل"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            count.CountDate = dto.CountDate == default ? DateTime.Now : dto.CountDate;
            count.WarehouseID = dto.WarehouseID;
            count.Notes = dto.Notes?.Trim() ?? string.Empty;

            _context.StockCountItems.RemoveRange(count.Items);
            count.Items.Clear();

            foreach (var line in dto.Items)
            {
                if (line.ItemID <= 0 || line.CountedQuantity < 0)
                    return BadRequest(ApiResponse.Fail("رقم صنف أو كمية جرد غير صالحة"));

                var item = await _context.InventoryItems.FindAsync(line.ItemID);
                if (item == null || !item.IsActive)
                    return BadRequest(ApiResponse.Fail("أحد الأصناف غير موجود أو غير مفعّل"));

                var systemQty = await GetStockAsync(line.ItemID, dto.WarehouseID);

                count.Items.Add(new StockCountItem
                {
                    ItemID = line.ItemID,
                    SystemQuantity = systemQty,
                    CountedQuantity = line.CountedQuantity,
                    UnitPrice = item.PurchasePrice,
                    Notes = line.Notes
                });
            }

            if (count.Items.Select(i => i.ItemID).Distinct().Count() != count.Items.Count)
                return BadRequest(ApiResponse.Fail("لا يمكن تكرار نفس الصنف في سند الجرد"));

            await AuditAsync("StockCountUpdated", "StockCount", count.StockCountID, $"تعديل سند جرد {count.StockCountNumber}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث سند الجرد بنجاح"));
        }

        // POST: api/warehouse/counts/5/post
        [HttpPost("counts/{id}/post")]
        public async Task<IActionResult> PostCount(int id)
        {
            var count = await _context.StockCounts
                .Include(c => c.Warehouse)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(c => c.StockCountID == id);

            if (count == null)
                return NotFound(ApiResponse.Fail("سند الجرد غير موجود"));

            if (count.Status == "Posted")
                return BadRequest(ApiResponse.Fail("سند الجرد مرحّل بالفعل"));

            if (count.Status == "Reversed")
                return BadRequest(ApiResponse.Fail("لا يمكن ترحيل سند جرد معكوس"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            // التحقق قبل الترحيل: الكميات النظامية لم تتغير بما يتعارض مع جرد الإخراج
            foreach (var line in count.Items)
            {
                if (line.CountedQuantity > line.SystemQuantity) continue;
                var shortage = line.SystemQuantity - line.CountedQuantity;
                if (shortage <= 0) continue;

                var current = await GetStockAsync(line.ItemID, count.WarehouseID);
                if (current < shortage)
                    return BadRequest(ApiResponse.Fail($"رصيد الصنف (أقل من نظامي الجرد) تغيّر منذ الجرد — أعد الجرد للصنف قبل الترحيل"));
            }

            var adjustments = new Dictionary<string, StockMovement>();
            var year = DateTime.Now.Year;
            var baseCount = await _context.StockMovements.CountAsync(m => m.MovementDate.Year == year);
            int seq = 0;

            foreach (var line in count.Items)
            {
                var diff = line.CountedQuantity - line.SystemQuantity;
                if (Math.Abs(diff) < 0.005m) continue;

                var type = diff > 0 ? "In" : "Out";
                var qty = Math.Abs(diff);

                if (!adjustments.TryGetValue(type, out var movement))
                {
                    movement = new StockMovement
                    {
                        MovementNumber = $"MV-{year}-{(baseCount + ++seq):0000}",
                        MovementType = type,
                        MovementDate = count.CountDate,
                        WarehouseID = count.WarehouseID,
                        ToWarehouseID = null,
                        ReferenceType = "StockCount",
                        ReferenceID = count.StockCountID,
                        Notes = $"تسوية جرد {count.StockCountNumber}",
                        Status = "Posted",
                        CreatedByUserID = userId,
                        CreatedAt = DateTime.Now,
                        PostedByUserID = userId,
                        PostedAt = DateTime.Now
                    };
                    _context.StockMovements.Add(movement);
                    adjustments[type] = movement;
                }

                movement.Items.Add(new StockMovementItem
                {
                    ItemID = line.ItemID,
                    Quantity = qty,
                    UnitPrice = line.UnitPrice,
                    Notes = $"جرد {count.StockCountNumber} (نظامي {line.SystemQuantity:N2} / فعلي {line.CountedQuantity:N2})"
                });
            }

            count.Status = "Posted";
            count.PostedAt = DateTime.Now;
            count.PostedByUserID = userId;

            await AuditAsync("StockCountPosted", "StockCount", count.StockCountID, $"ترحيل سند جرد {count.StockCountNumber} مع تسوية أرصدة تلقائية");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم ترحيل سند الجرد {count.StockCountNumber} وإنشاء سندات التسوية تلقائياً"));
        }

        // POST: api/warehouse/counts/5/reverse
        [HttpPost("counts/{id}/reverse")]
        public async Task<IActionResult> ReverseCount(int id)
        {
            var count = await _context.StockCounts
                .Include(c => c.Warehouse)
                .FirstOrDefaultAsync(c => c.StockCountID == id);

            if (count == null)
                return NotFound(ApiResponse.Fail("سند الجرد غير موجود"));

            if (count.Status != "Posted")
                return BadRequest(ApiResponse.Fail("يُعكس فقط سند الجرد المرحّل"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            // عكس سندات التسوية المرتبطة
            var linked = await _context.StockMovements
                .Include(m => m.Items)
                .Where(m => m.ReferenceType == "StockCount" && m.ReferenceID == count.StockCountID && m.Status == "Posted")
                .ToListAsync();

            foreach (var movement in linked)
            {
                if (movement.MovementType == "In")
                {
                    foreach (var line in movement.Items)
                    {
                        var current = await GetStockAsync(line.ItemID, movement.WarehouseID);
                        if (line.Quantity > current)
                            return BadRequest(ApiResponse.Fail($"لا يمكن عكس سند الجرد: رصيد الصنف غير كافٍ في المخزن ({current:N2})"));
                    }
                }
                movement.Status = "Reversed";
            }

            count.Status = "Reversed";
            count.ReversedAt = DateTime.Now;
            count.ReversedByUserID = userId;

            await AuditAsync("StockCountReversed", "StockCount", count.StockCountID, $"عكس سند جرد {count.StockCountNumber}");
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم عكس سند الجرد {count.StockCountNumber} وعكس سندات التسوية"));
        }

        // ============================================================
        //  تصدير Excel / CSV
        // ============================================================

        // GET: api/warehouse/export/stock?warehouseId=...&categoryId=...&format=xlsx|csv
        [HttpGet("export/stock")]
        public async Task<IActionResult> ExportStock([FromQuery] int? warehouseId, [FromQuery] int? categoryId, [FromQuery] string? format)
        {
            var result = await ComputeStockRowsAsync();

            var rows = result
                .Where(r => !warehouseId.HasValue || r.WarehouseID == warehouseId.Value)
                .Where(r => !categoryId.HasValue || r.CategoryID == categoryId.Value)
                .OrderBy(r => r.ItemCode)
                .ToList();

            var headers = new[] { "كود الصنف", "اسم الصنف", "الفئة", "رقم المخزن", "الوحدة", "الكمية المتاحة", "حد إعادة الطلب" };
            var data = rows.Select(r => new object[]
            {
                r.ItemCode, r.ItemNameAr, r.CategoryNameAr, r.WarehouseID, r.Unit, r.Quantity, r.ReorderLevel
            }).ToList();

            return await ExportAsync(format, "الأرصدة", "stock", headers, data);
        }

        // GET: api/warehouse/export/movements?type=...&status=...&from=...&to=...&format=xlsx|csv
        [HttpGet("export/movements")]
        public async Task<IActionResult> ExportMovements(
            [FromQuery] string? type,
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? format)
        {
            var query = _context.StockMovements.AsQueryable();

            if (!string.IsNullOrEmpty(type))
                query = query.Where(m => m.MovementType == type);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status == status);

            if (from.HasValue)
                query = query.Where(m => m.MovementDate >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(m => m.MovementDate < to.Value.Date.AddDays(1));

            var movements = await query
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.MovementID)
                .Select(m => new
                {
                    m.MovementNumber,
                    m.MovementType,
                    m.MovementDate,
                    m.WarehouseID,
                    m.ReferenceType,
                    m.Notes,
                    m.Status,
                    CreatedByName = m.CreatedByUser != null ? m.CreatedByUser.FullName : null,
                    ItemsCount = m.Items.Count,
                    TotalQuantity = m.Items.Sum(i => i.Quantity),
                    TotalValue = m.Items.Sum(i => i.Quantity * i.UnitPrice)
                })
                .ToListAsync();

            var headers = new[] { "رقم السند", "النوع", "التاريخ", "رقم المخزن", "المرجع", "البيان", "الحالة", "بواسطة", "عدد الأصناف", "الكمية", "القيمة (د.ل)" };
            var data = movements.Select(m => new object[]
            {
                m.MovementNumber, m.MovementType, m.MovementDate.ToString("yyyy-MM-dd HH:mm"), m.WarehouseID,
                m.ReferenceType, m.Notes, m.Status, m.CreatedByName, m.ItemsCount, m.TotalQuantity, m.TotalValue
            }).ToList();

            return await ExportAsync(format, "الحركة", "movements", headers, data);
        }

        // GET: api/warehouse/export/items?categoryId=...&search=...&format=xlsx|csv
        [HttpGet("export/items")]
        public async Task<IActionResult> ExportItems(
            [FromQuery] int? categoryId,
            [FromQuery] string? search,
            [FromQuery] string? format)
        {
            var query = _context.InventoryItems.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryID == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.ItemNameAr.Contains(search) || i.ItemName.Contains(search) || i.ItemCode.Contains(search));

            var items = await query
                .OrderBy(i => i.ItemCode)
                .Select(i => new
                {
                    i.ItemCode,
                    i.ItemNameAr,
                    i.ItemName,
                    CategoryNameAr = i.Category.CategoryNameAr,
                    i.Unit,
                    i.PurchasePrice,
                    i.SellingPrice,
                    i.ReorderLevel,
                    i.Manufacturer,
                    i.ExpiryDate,
                    i.IsActive
                })
                .ToListAsync();

            var headers = new[] { "الكود", "الاسم", "الاسم بالإنجليزية", "الفئة", "الوحدة", "سعر الشراء", "سعر البيع", "حد إعادة الطلب", "المصنّع", "تاريخ الانتهاء", "مفعّل" };
            var data = items.Select(i => new object[]
            {
                i.ItemCode, i.ItemNameAr, i.ItemName, i.CategoryNameAr, i.Unit, i.PurchasePrice, i.SellingPrice,
                i.ReorderLevel, i.Manufacturer ?? "", i.ExpiryDate?.ToString("yyyy-MM-dd") ?? "", i.IsActive ? "نعم" : "لا"
            }).ToList();

            return await ExportAsync(format, "الأصناف", "items", headers, data);
        }

        // GET: api/warehouse/export/counts?status=...&from=...&to=...&format=xlsx|csv
        [HttpGet("export/counts")]
        public async Task<IActionResult> ExportCounts(
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? format)
        {
            var query = _context.StockCounts.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(c => c.Status == status);

            if (from.HasValue)
                query = query.Where(c => c.CountDate >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(c => c.CountDate < to.Value.Date.AddDays(1));

            var counts = await query
                .OrderByDescending(c => c.CountDate)
                .ThenByDescending(c => c.StockCountID)
                .Select(c => new
                {
                    c.StockCountNumber,
                    c.CountDate,
                    c.WarehouseID,
                    c.Status,
                    c.Notes,
                    ItemsCount = c.Items.Count,
                    TotalSystem = c.Items.Sum(i => i.SystemQuantity),
                    TotalCounted = c.Items.Sum(i => i.CountedQuantity),
                    TotalDifference = c.Items.Sum(i => i.CountedQuantity - i.SystemQuantity),
                    CreatedByName = c.CreatedByUser != null ? c.CreatedByUser.FullName : null
                })
                .ToListAsync();

            var headers = new[] { "رقم الجرد", "التاريخ", "رقم المخزن", "الحالة", "البيان", "عدد الأصناف", "نظامي", "فعلي", "الفرق", "بواسطة" };
            var data = counts.Select(c => new object[]
            {
                c.StockCountNumber, c.CountDate.ToString("yyyy-MM-dd"), c.WarehouseID, c.Status, c.Notes,
                c.ItemsCount, c.TotalSystem, c.TotalCounted, c.TotalDifference, c.CreatedByName
            }).ToList();

            return await ExportAsync(format, "الجرد", "counts", headers, data);
        }

        private async Task<IActionResult> ExportAsync(string? format, string sheetName, string fileNameBase, string[] headers, IReadOnlyList<object[]> rows)
        {
            var fmt = (format ?? "xlsx").ToLowerInvariant();
            if (fmt == "csv")
                return CsvResult(headers, rows, fileNameBase);
            return XlsxResult(sheetName, headers, rows, fileNameBase);
        }

        private IActionResult XlsxResult(string sheetName, string[] headers, IReadOnlyList<object[]> rows, string fileNameBase)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            for (int c = 0; c < headers.Length; c++)
                ws.Cell(1, c + 1).Value = headers[c];

            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#4361EE");
            ws.Row(1).Style.Font.FontColor = XLColor.White;

            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Length; c++)
                {
                    var value = rows[r][c];
                    if (value is decimal || value is int || value is double || value is float || value is long)
                        ws.Cell(r + 2, c + 1).Value = Convert.ToDecimal(value);
                    else
                        ws.Cell(r + 2, c + 1).Value = value?.ToString() ?? "";
                }
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            var name = $"{fileNameBase}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name);
        }

        private IActionResult CsvResult(string[] headers, IReadOnlyList<object[]> rows, string fileNameBase)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers.Select(CsvEscape)));

            foreach (var row in rows)
                sb.AppendLine(string.Join(",", row.Select(v => CsvEscape(v?.ToString() ?? ""))));

            var preamble = Encoding.UTF8.GetPreamble();
            var body = Encoding.UTF8.GetBytes(sb.ToString());
            var bytes = preamble.Concat(body).ToArray();

            var name = $"{fileNameBase}_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            return File(bytes, "text/csv; charset=utf-8", name);
        }

        private static string CsvEscape(string value)
            => value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r')
                ? "\"" + value.Replace("\"", "\"\"") + "\""
                : value;

        // ============================================================
        //  دوال مساعدة
        // ============================================================

        private async Task<decimal> GetStockAsync(int itemId, int warehouseId)
        {
            var lines = await _context.StockMovementItems
                .Include(i => i.Movement)
                .Where(i => i.ItemID == itemId && i.Movement.Status == "Posted")
                .ToListAsync();

            decimal stock = 0;
            foreach (var line in lines)
            {
                if (line.Movement.MovementType == "In")
                {
                    if (line.Movement.WarehouseID == warehouseId) stock += line.Quantity;
                }
                else if (line.Movement.MovementType == "Out")
                {
                    if (line.Movement.WarehouseID == warehouseId) stock -= line.Quantity;
                }
                else if (line.Movement.MovementType == "Transfer")
                {
                    if (line.Movement.WarehouseID == warehouseId) stock -= line.Quantity;
                    if (line.Movement.ToWarehouseID == warehouseId) stock += line.Quantity;
                }
            }
            return stock;
        }

        private async Task<string> GenerateMovementNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _context.StockMovements.CountAsync(m => m.MovementDate.Year == year);
            return $"MV-{year}-{(count + 1):0000}";
        }

        private async Task<string> GenerateCountNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _context.StockCounts.CountAsync(c => c.CountDate.Year == year);
            return $"CNT-{year}-{(count + 1):0000}";
        }

        private async Task AuditAsync(string action, string entityType, int entityId, string details)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = action,
                EntityType = entityType,
                EntityID = entityId,
                UserID = userId,
                Details = details,
                Timestamp = DateTime.Now
            });
        }

        private static object BuildCategoryNode(InventoryCategory category, List<InventoryCategory> all)
        {
            var children = all
                .Where(c => c.ParentCategoryID == category.CategoryID)
                .Select(c => BuildCategoryNode(c, all))
                .ToList();

            return new
            {
                category.CategoryID,
                category.CategoryName,
                category.CategoryNameAr,
                category.ParentCategoryID,
                category.IsActive,
                Children = children
            };
        }
    }
}

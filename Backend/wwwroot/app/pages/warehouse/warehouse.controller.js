(function() {
    'use strict';

    angular.module('medicalApp').controller('WarehouseController', WarehouseController);

    WarehouseController.$inject = ['WarehouseService', 'AuthService', 'toastr'];

    function WarehouseController(WarehouseService, AuthService, toastr) {
        var vm = this;

        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.isAdmin = vm.role === 'Admin';

        // Tabs: movements | warehouses | items | stock | lowStock
        vm.activeTab = 'movements';
        vm.setTab = setTab;

        // ==== Warehouses ====
        vm.warehouses = [];
        vm.loadWarehouses = loadWarehouses;
        vm.warehouseForm = null;
        vm.editingWarehouse = null;
        vm.showWarehouseModal = false;
        vm.savingWarehouse = false;
        vm.openWarehouseModal = openWarehouseModal;
        vm.closeWarehouseModal = closeWarehouseModal;
        vm.saveWarehouse = saveWarehouse;
        vm.deleteWarehouse = deleteWarehouse;

        // ==== Categories ====
        vm.categories = [];
        vm.flatCategories = [];
        vm.loadCategories = loadCategories;
        vm.categoryForm = null;
        vm.editingCategory = null;
        vm.showCategoryModal = false;
        vm.savingCategory = false;
        vm.openCategoryModal = openCategoryModal;
        vm.closeCategoryModal = closeCategoryModal;
        vm.saveCategory = saveCategory;

        // ==== Items ====
        vm.items = [];
        vm.allItems = [];
        vm.itemFilterCategory = '';
        vm.itemSearch = '';
        vm.itemPage = 1;
        vm.itemPageSize = 15;
        vm.itemTotalPages = 1;
        vm.itemsLoading = false;
        vm.loadItems = loadItems;
        vm.applyItemFilters = applyItemFilters;
        vm.itemPageChange = itemPageChange;
        vm.itemForm = null;
        vm.editingItem = null;
        vm.showItemModal = false;
        vm.savingItem = false;
        vm.openItemModal = openItemModal;
        vm.closeItemModal = closeItemModal;
        vm.saveItem = saveItem;

        // ==== Movements ====
        vm.movements = [];
        vm.filterType = '';
        vm.filterStatus = '';
        vm.filterFrom = '';
        vm.filterTo = '';
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalPages = 1;
        vm.movementsLoading = false;
        vm.loadMovements = loadMovements;
        vm.applyMovementFilters = applyMovementFilters;
        vm.pageChange = pageChange;

        // New movement modal
        vm.showMovementModal = false;
        vm.movementForm = null;
        vm.savingMovement = false;
        vm.openMovementModal = openMovementModal;
        vm.closeMovementModal = closeMovementModal;
        vm.onMovementTypeChange = onMovementTypeChange;
        vm.addMovementLine = addMovementLine;
        vm.removeMovementLine = removeMovementLine;
        vm.saveMovement = saveMovement;

        // Movement detail modal
        vm.showMovementDetail = false;
        vm.currentMovement = null;
        vm.openMovementDetail = openMovementDetail;
        vm.closeMovementDetail = closeMovementDetail;
        vm.postMovement = postMovement;
        vm.reverseMovement = reverseMovement;

        // ==== Stock ====
        vm.stock = [];
        vm.stockWarehouseID = '';
        vm.stockLoading = false;
        vm.loadStock = loadStock;

        // ==== Low Stock ====
        vm.lowStock = [];
        vm.lowStockLoading = false;
        vm.loadLowStock = loadLowStock;

        // ==== Stock Counts ====
        vm.counts = [];
        vm.countFilterStatus = '';
        vm.countPage = 1;
        vm.countPageSize = 10;
        vm.countTotalPages = 1;
        vm.countsLoading = false;
        vm.loadCounts = loadCounts;
        vm.applyCountFilters = applyCountFilters;
        vm.countPageChange = countPageChange;

        // New count modal
        vm.showCountModal = false;
        vm.countForm = null;
        vm.savingCount = false;
        vm.openCountModal = openCountModal;
        vm.closeCountModal = closeCountModal;
        vm.onCountWarehouseChange = onCountWarehouseChange;
        vm.addCountLine = addCountLine;
        vm.removeCountLine = removeCountLine;
        vm.countLineSystemQty = countLineSystemQty;
        vm.saveCount = saveCount;
        vm.stockByItem = {};

        // Count detail modal
        vm.showCountDetail = false;
        vm.currentCount = null;
        vm.openCountDetail = openCountDetail;
        vm.closeCountDetail = closeCountDetail;
        vm.postCount = postCount;
        vm.reverseCount = reverseCount;

        // ==== Exports ====
        vm.exportStock = exportStock;
        vm.exportMovements = exportMovements;
        vm.exportItems = exportItems;
        vm.exportCounts = exportCounts;

        activate();

        function activate() {
            loadWarehouses();
            loadMovements();
            loadCategories();
            loadItems();
            loadAllItems();
        }

        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'warehouses') loadWarehouses();
            if (tab === 'movements') loadMovements();
            if (tab === 'items') loadCategories();
            if (tab === 'stock') loadStock();
            if (tab === 'lowStock') loadLowStock();
            if (tab === 'counts') loadCounts();
        }

        // ============================================================
        //  Warehouses
        // ============================================================
        function loadWarehouses() {
            WarehouseService.getWarehouses()
                .then(function(res) {
                    if (res.success) vm.warehouses = res.data || [];
                })
                .catch(function() {
                    toastr.error('فشل تحميل المخازن');
                });
        }

        function openWarehouseModal(warehouse) {
            if (!vm.isAdmin) return;
            vm.editingWarehouse = warehouse || null;
            vm.warehouseForm = vm.editingWarehouse ? {
                warehouseName: vm.editingWarehouse.warehouseName || '',
                warehouseNameAr: vm.editingWarehouse.warehouseNameAr,
                warehouseCode: vm.editingWarehouse.warehouseCode,
                location: vm.editingWarehouse.location || '',
                isActive: vm.editingWarehouse.isActive
            } : {
                warehouseName: '',
                warehouseNameAr: '',
                warehouseCode: '',
                location: '',
                isActive: true
            };
            vm.showWarehouseModal = true;
        }

        function closeWarehouseModal() {
            vm.showWarehouseModal = false;
            vm.editingWarehouse = null;
        }

        function saveWarehouse() {
            var form = vm.warehouseForm;
            if (!form.warehouseNameAr || !form.warehouseCode) {
                toastr.warning('اسم المخزن بالعربية والكود مطلوبان');
                return;
            }

            var payload = {
                warehouseName: form.warehouseName || form.warehouseNameAr,
                warehouseNameAr: form.warehouseNameAr,
                warehouseCode: form.warehouseCode,
                location: form.location || null,
                isActive: !!form.isActive
            };

            vm.savingWarehouse = true;
            var promise = vm.editingWarehouse
                ? WarehouseService.updateWarehouse(vm.editingWarehouse.warehouseID, payload)
                : WarehouseService.createWarehouse(payload);

            promise
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ المخزن بنجاح');
                        vm.closeWarehouseModal();
                        loadWarehouses();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حفظ المخزن');
                })
                .finally(function() {
                    vm.savingWarehouse = false;
                });
        }

        function deleteWarehouse(warehouse) {
            if (!vm.isAdmin) return;
            if (!confirm('هل تريد حذف المخزن "' + warehouse.warehouseNameAr + '"؟')) return;

            WarehouseService.deleteWarehouse(warehouse.warehouseID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حذف المخزن');
                        loadWarehouses();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حذف المخزن');
                });
        }

        // ============================================================
        //  Categories
        // ============================================================
        function loadCategories() {
            WarehouseService.getCategories()
                .then(function(res) {
                    if (res.success) vm.categories = res.data || [];
                })
                .catch(function() {
                    toastr.error('فشل تحميل الفئات');
                });

            WarehouseService.getCategoriesFlat()
                .then(function(res) {
                    if (res.success) vm.flatCategories = res.data || [];
                })
                .catch(function() {});
        }

        function openCategoryModal(category) {
            if (!vm.isAdmin) return;
            vm.editingCategory = category || null;
            vm.categoryForm = vm.editingCategory ? {
                categoryName: vm.editingCategory.categoryName || '',
                categoryNameAr: vm.editingCategory.categoryNameAr,
                parentCategoryID: vm.editingCategory.parentCategoryID || null,
                isActive: vm.editingCategory.isActive
            } : {
                categoryName: '',
                categoryNameAr: '',
                parentCategoryID: null,
                isActive: true
            };
            vm.showCategoryModal = true;
        }

        function closeCategoryModal() {
            vm.showCategoryModal = false;
            vm.editingCategory = null;
        }

        function saveCategory() {
            var form = vm.categoryForm;
            if (!form.categoryNameAr) {
                toastr.warning('اسم الفئة بالعربية مطلوب');
                return;
            }

            var payload = {
                categoryName: form.categoryName || form.categoryNameAr,
                categoryNameAr: form.categoryNameAr,
                parentCategoryID: form.parentCategoryID ? Number(form.parentCategoryID) : null,
                isActive: !!form.isActive
            };

            vm.savingCategory = true;
            var promise = vm.editingCategory
                ? WarehouseService.updateCategory(vm.editingCategory.categoryID, payload)
                : WarehouseService.createCategory(payload);

            promise
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ الفئة بنجاح');
                        vm.closeCategoryModal();
                        loadCategories();
                        loadItems();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حفظ الفئة');
                })
                .finally(function() {
                    vm.savingCategory = false;
                });
        }

        // ============================================================
        //  Items
        // ============================================================
        function loadItems() {
            vm.itemsLoading = true;
            var params = {
                categoryId: vm.itemFilterCategory || null,
                search: vm.itemSearch || null,
                page: vm.itemPage,
                pageSize: vm.itemPageSize
            };

            WarehouseService.getItems(params)
                .then(function(res) {
                    vm.items = res.data || [];
                    vm.itemTotalPages = Math.ceil(res.totalCount / vm.itemPageSize) || 1;
                })
                .catch(function() {
                    toastr.error('فشل تحميل الأصناف');
                })
                .finally(function() {
                    vm.itemsLoading = false;
                });
        }

        function applyItemFilters() {
            vm.itemPage = 1;
            loadItems();
        }

        function itemPageChange(dir) {
            vm.itemPage += dir;
            if (vm.itemPage < 1) vm.itemPage = 1;
            if (vm.itemPage > vm.itemTotalPages) vm.itemPage = vm.itemTotalPages;
            loadItems();
        }

        function loadAllItems() {
            var collected = [];
            var page = 1;
            var pageSize = 100;
            var next = function() {
                WarehouseService.getItems({ page: page, pageSize: pageSize })
                    .then(function(res) {
                        collected = collected.concat(res.data || []);
                        if (res.data && res.data.length === pageSize) {
                            page += 1;
                            next();
                        } else {
                            vm.allItems = collected;
                        }
                    })
                    .catch(function() {});
            };
            next();
        }

        function openItemModal(item) {
            if (!vm.isAdmin) return;
            vm.editingItem = item || null;
            vm.itemForm = vm.editingItem ? {
                itemCode: vm.editingItem.itemCode,
                itemNameAr: vm.editingItem.itemNameAr,
                itemName: vm.editingItem.itemName || '',
                categoryID: vm.editingItem.categoryID,
                unit: vm.editingItem.unit,
                purchasePrice: vm.editingItem.purchasePrice,
                sellingPrice: vm.editingItem.sellingPrice,
                reorderLevel: vm.editingItem.reorderLevel,
                manufacturer: vm.editingItem.manufacturer || '',
                expiryDate: vm.editingItem.expiryDate ? new Date(vm.editingItem.expiryDate) : null,
                isActive: vm.editingItem.isActive
            } : {
                itemCode: '',
                itemNameAr: '',
                itemName: '',
                categoryID: vm.flatCategories.length > 0 ? vm.flatCategories[0].categoryID : null,
                unit: 'قطعة',
                purchasePrice: 0,
                sellingPrice: 0,
                reorderLevel: 10,
                manufacturer: '',
                expiryDate: null,
                isActive: true
            };
            vm.showItemModal = true;
        }

        function closeItemModal() {
            vm.showItemModal = false;
            vm.editingItem = null;
        }

        function saveItem() {
            var form = vm.itemForm;
            if (!form.itemCode || !form.itemNameAr || !form.categoryID) {
                toastr.warning('كود الصنف واسمه بالعربية والفئة مطلوبة');
                return;
            }

            var payload = {
                itemCode: form.itemCode,
                itemNameAr: form.itemNameAr,
                itemName: form.itemName || form.itemNameAr,
                categoryID: Number(form.categoryID),
                unit: form.unit || 'قطعة',
                purchasePrice: Number(form.purchasePrice) || 0,
                sellingPrice: Number(form.sellingPrice) || 0,
                reorderLevel: Number(form.reorderLevel) || 0,
                manufacturer: form.manufacturer || null,
                expiryDate: form.expiryDate ? new Date(form.expiryDate).toISOString() : null,
                isActive: !!form.isActive
            };

            vm.savingItem = true;
            var promise = vm.editingItem
                ? WarehouseService.updateItem(vm.editingItem.itemID, payload)
                : WarehouseService.createItem(payload);

            promise
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ الصنف بنجاح');
                        vm.closeItemModal();
                        loadItems();
                        loadAllItems();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حفظ الصنف');
                })
                .finally(function() {
                    vm.savingItem = false;
                });
        }

        // ============================================================
        //  Movements
        // ============================================================
        function loadMovements() {
            vm.movementsLoading = true;
            var params = {
                type: vm.filterType || null,
                status: vm.filterStatus || null,
                from: vm.filterFrom || null,
                to: vm.filterTo || null,
                page: vm.page,
                pageSize: vm.pageSize
            };

            WarehouseService.getMovements(params)
                .then(function(res) {
                    vm.movements = res.data || [];
                    vm.totalPages = Math.ceil(res.totalCount / vm.pageSize) || 1;
                })
                .catch(function() {
                    toastr.error('فشل تحميل سندات المخزن');
                })
                .finally(function() {
                    vm.movementsLoading = false;
                });
        }

        function applyMovementFilters() {
            vm.page = 1;
            loadMovements();
        }

        function pageChange(dir) {
            vm.page += dir;
            if (vm.page < 1) vm.page = 1;
            if (vm.page > vm.totalPages) vm.page = vm.totalPages;
            loadMovements();
        }

        function openMovementModal() {
            vm.movementForm = {
                movementDate: new Date(),
                movementType: 'In',
                warehouseID: vm.warehouses.length > 0 ? vm.warehouses[0].warehouseID : null,
                toWarehouseID: null,
                referenceType: 'Adjustment',
                notes: '',
                items: [newMovementLine()]
            };
            onMovementTypeChange();
            vm.showMovementModal = true;
        }

        function newMovementLine() {
            return {
                itemID: null,
                quantity: 1,
                unitPrice: 0,
                notes: ''
            };
        }

        function closeMovementModal() {
            vm.showMovementModal = false;
        }

        function onMovementTypeChange() {
            if (vm.movementForm.movementType !== 'Transfer') {
                vm.movementForm.toWarehouseID = null;
            }
        }

        function addMovementLine() {
            vm.movementForm.items.push(newMovementLine());
        }

        function removeMovementLine(index) {
            if (vm.movementForm.items.length <= 1) {
                toastr.warning('يجب إدخال صنف واحد على الأقل');
                return;
            }
            vm.movementForm.items.splice(index, 1);
        }

        function saveMovement() {
            var form = vm.movementForm;
            if (!form.warehouseID) {
                toastr.warning('اختر المخزن');
                return;
            }
            if (form.movementType === 'Transfer' && !form.toWarehouseID) {
                toastr.warning('حدد مخزن التحويل إليه');
                return;
            }
            var validLines = form.items.filter(function(line) {
                return line.itemID && line.quantity > 0;
            });
            if (validLines.length === 0) {
                toastr.warning('أدخل صنفاً واحداً على الأقل بكمية صحيحة');
                return;
            }

            var payload = {
                movementDate: new Date(form.movementDate).toISOString(),
                movementType: form.movementType,
                warehouseID: Number(form.warehouseID),
                toWarehouseID: form.movementType === 'Transfer' ? Number(form.toWarehouseID) : null,
                referenceType: form.referenceType || 'Adjustment',
                notes: form.notes || '',
                items: validLines.map(function(line) {
                    return {
                        itemID: Number(line.itemID),
                        quantity: Number(line.quantity),
                        unitPrice: Number(line.unitPrice) || 0,
                        notes: line.notes || null
                    };
                })
            };

            vm.savingMovement = true;
            WarehouseService.createMovement(payload)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إنشاء السند بنجاح');
                        vm.closeMovementModal();
                        loadMovements();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء إنشاء السند');
                })
                .finally(function() {
                    vm.savingMovement = false;
                });
        }

        function openMovementDetail(movement) {
            WarehouseService.getMovement(movement.movementID)
                .then(function(res) {
                    if (res.success) {
                        vm.currentMovement = res.data;
                        vm.showMovementDetail = true;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل تفاصيل السند');
                });
        }

        function closeMovementDetail() {
            vm.showMovementDetail = false;
            vm.currentMovement = null;
        }

        function postMovement(movement) {
            if (!confirm('سيتم ترحيل سند المخزن ' + movement.movementNumber + ' وتحديث الأرصدة. هل تريد المتابعة؟')) return;

            WarehouseService.postMovement(movement.movementID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم ترحيل السند');
                        vm.closeMovementDetail();
                        loadMovements();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء ترحيل السند');
                });
        }

        function reverseMovement(movement) {
            if (!confirm('سيتم عكس سند المخزن ' + movement.movementNumber + ' وإرجاع الأرصدة. هل تريد المتابعة؟')) return;

            WarehouseService.reverseMovement(movement.movementID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم عكس السند');
                        vm.closeMovementDetail();
                        loadMovements();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء عكس السند');
                });
        }

        // ============================================================
        //  Stock
        // ============================================================
        function loadStock() {
            vm.stockLoading = true;
            var params = {
                warehouseId: vm.stockWarehouseID || null
            };

            WarehouseService.getStock(params)
                .then(function(res) {
                    if (res.success) {
                        vm.stock = res.data || [];
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل الكميات والأرصدة');
                })
                .finally(function() {
                    vm.stockLoading = false;
                });
        }

        // ============================================================
        //  Low Stock
        // ============================================================
        function loadLowStock() {
            vm.lowStockLoading = true;
            WarehouseService.getLowStock()
                .then(function(res) {
                    if (res.success) {
                        vm.lowStock = res.data || [];
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل تنبيهات انخفاض المخزون');
                })
                .finally(function() {
                    vm.lowStockLoading = false;
                });
        }

        // ============================================================
        //  Stock Counts (الجرد الدوري)
        // ============================================================
        function loadCounts() {
            vm.countsLoading = true;
            var params = {
                status: vm.countFilterStatus || null,
                page: vm.countPage,
                pageSize: vm.countPageSize
            };

            WarehouseService.getCounts(params)
                .then(function(res) {
                    vm.counts = res.data || [];
                    vm.countTotalPages = Math.ceil(res.totalCount / vm.countPageSize) || 1;
                })
                .catch(function() {
                    toastr.error('فشل تحميل سندات الجرد');
                })
                .finally(function() {
                    vm.countsLoading = false;
                });
        }

        function applyCountFilters() {
            vm.countPage = 1;
            loadCounts();
        }

        function countPageChange(dir) {
            vm.countPage += dir;
            if (vm.countPage < 1) vm.countPage = 1;
            if (vm.countPage > vm.countTotalPages) vm.countPage = vm.countTotalPages;
            loadCounts();
        }

        function openCountModal(count) {
            vm.editingCount = count || null;
            if (count) {
                WarehouseService.getCount(count.stockCountID)
                    .then(function(res) {
                        if (res.success) {
                            vm.countForm = {
                                countDate: res.data.countDate ? new Date(res.data.countDate) : new Date(),
                                warehouseID: res.data.warehouseID,
                                notes: res.data.notes || '',
                                items: res.data.items.map(function(line) {
                                    return {
                                        itemID: line.itemID,
                                        countedQuantity: line.countedQuantity,
                                        notes: line.notes || ''
                                    };
                                })
                            };
                            onCountWarehouseChange();
                            vm.showCountModal = true;
                        }
                    })
                    .catch(function() {
                        toastr.error('فشل تحميل بيانات الجرد');
                    });
                return;
            }

            vm.countForm = {
                countDate: new Date(),
                warehouseID: vm.warehouses.length > 0 ? vm.warehouses[0].warehouseID : null,
                notes: '',
                items: [newCountLine()]
            };
            onCountWarehouseChange();
            vm.showCountModal = true;
        }

        function newCountLine() {
            return { itemID: null, countedQuantity: 0, notes: '' };
        }

        function closeCountModal() {
            vm.showCountModal = false;
            vm.editingCount = null;
        }

        function onCountWarehouseChange() {
            if (!vm.countForm || !vm.countForm.warehouseID) return;
            WarehouseService.getStock({ warehouseId: vm.countForm.warehouseID })
                .then(function(res) {
                    if (res.success) {
                        vm.stockByItem = {};
                        (res.data || []).forEach(function(row) {
                            vm.stockByItem[row.itemID] = row.quantity;
                        });
                    }
                })
                .catch(function() {});
        }

        function countLineSystemQty(itemID) {
            if (!itemID) return 0;
            return vm.stockByItem[Number(itemID)] || 0;
        }

        function addCountLine() {
            vm.countForm.items.push(newCountLine());
        }

        function removeCountLine(index) {
            if (vm.countForm.items.length <= 1) {
                toastr.warning('يجب إدخال صنف واحد على الأقل');
                return;
            }
            vm.countForm.items.splice(index, 1);
        }

        function saveCount() {
            var form = vm.countForm;
            if (!form.warehouseID) {
                toastr.warning('اختر المخزن');
                return;
            }
            var validLines = form.items.filter(function(line) {
                return line.itemID && line.countedQuantity >= 0;
            });
            if (validLines.length === 0) {
                toastr.warning('أدخل صنفاً واحداً على الأقل');
                return;
            }
            var seen = {};
            for (var i = 0; i < validLines.length; i++) {
                var key = Number(validLines[i].itemID);
                if (seen[key]) {
                    toastr.warning('لا يمكن تكرار نفس الصنف في الجرد');
                    return;
                }
                seen[key] = true;
            }

            var payload = {
                countDate: new Date(form.countDate).toISOString(),
                warehouseID: Number(form.warehouseID),
                notes: form.notes || '',
                items: validLines.map(function(line) {
                    return {
                        itemID: Number(line.itemID),
                        countedQuantity: Number(line.countedQuantity) || 0,
                        notes: line.notes || null
                    };
                })
            };

            vm.savingCount = true;
            var promise = vm.editingCount
                ? WarehouseService.updateCount(vm.editingCount.stockCountID, payload)
                : WarehouseService.createCount(payload);
            promise
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ سند الجرد');
                        vm.closeCountModal();
                        loadCounts();
                        loadMovements();
                        loadStock();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حفظ الجرد');
                })
                .finally(function() {
                    vm.savingCount = false;
                });
        }

        function openCountDetail(count) {
            WarehouseService.getCount(count.stockCountID)
                .then(function(res) {
                    if (res.success) {
                        vm.currentCount = res.data;
                        vm.showCountDetail = true;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل تفاصيل الجرد');
                });
        }

        function closeCountDetail() {
            vm.showCountDetail = false;
            vm.currentCount = null;
        }

        function postCount(count) {
            if (!confirm('سيتم ترحيل الجرد ' + count.stockCountNumber + ' وإنشاء سندات تسوية تلقائية للأرصدة. هل تريد المتابعة؟')) return;

            WarehouseService.postCount(count.stockCountID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم ترحيل الجرد');
                        vm.closeCountDetail();
                        loadCounts();
                        loadMovements();
                        loadStock();
                        loadLowStock();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء ترحيل الجرد');
                });
        }

        function reverseCount(count) {
            if (!confirm('سيتم عكس الجرد ' + count.stockCountNumber + ' وعكس سندات التسوية وإرجاع الأرصدة. هل تريد المتابعة؟')) return;

            WarehouseService.reverseCount(count.stockCountID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم عكس الجرد');
                        vm.closeCountDetail();
                        loadCounts();
                        loadMovements();
                        loadStock();
                        loadLowStock();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء عكس الجرد');
                });
        }

        // ============================================================
        //  Exports (Excel / CSV)
        // ============================================================
        function exportStock(format) {
            WarehouseService.exportStock({ warehouseId: vm.stockWarehouseID || null, format: format || 'xlsx' })
                .catch(function() { toastr.error('فشل تصدير الأرصدة'); });
        }

        function exportMovements(format) {
            WarehouseService.exportMovements({
                type: vm.filterType || null,
                status: vm.filterStatus || null,
                from: vm.filterFrom || null,
                to: vm.filterTo || null,
                format: format || 'xlsx'
            }).catch(function() { toastr.error('فشل تصدير الحركة'); });
        }

        function exportItems(format) {
            WarehouseService.exportItems({
                categoryId: vm.itemFilterCategory || null,
                search: vm.itemSearch || null,
                format: format || 'xlsx'
            }).catch(function() { toastr.error('فشل تصدير الأصناف'); });
        }

        function exportCounts(format) {
            WarehouseService.exportCounts({
                status: vm.countFilterStatus || null,
                format: format || 'xlsx'
            }).catch(function() { toastr.error('فشل تصدير الجرد'); });
        }

        // ============================================================
        //  Helpers
        // ============================================================
        vm.movementTypeAr = movementTypeAr;
        vm.statusAr = statusAr;
        vm.statusBadgeClass = statusBadgeClass;
        vm.categoryPath = categoryPath;
        vm.warehouseNameAr = warehouseNameAr;

        function warehouseNameAr(id) {
            var name = id;
            for (var i = 0; i < vm.warehouses.length; i++) {
                if (vm.warehouses[i].warehouseID === Number(id)) {
                    name = vm.warehouses[i].warehouseNameAr;
                    break;
                }
            }
            return name;
        }

        function movementTypeAr(type) {
            var map = {
                'In': 'سند إدخال',
                'Out': 'سند إخراج',
                'Transfer': 'سند تحويل'
            };
            return map[type] || type;
        }

        function statusAr(status) {
            var map = {
                'Draft': 'مسودة',
                'Posted': 'مرحّل',
                'Reversed': 'معكوس'
            };
            return map[status] || status;
        }

        function statusBadgeClass(status) {
            var map = {
                'Draft': 'acc-badge-draft',
                'Posted': 'acc-badge-posted',
                'Reversed': 'acc-badge-reversed'
            };
            return map[status] || 'acc-badge-draft';
        }

        function categoryPath(categoryID) {
            var names = [];
            var id = categoryID;
            var guard = 0;
            while (id && guard < 10) {
                var found = null;
                for (var i = 0; i < vm.flatCategories.length; i++) {
                    if (vm.flatCategories[i].categoryID === Number(id)) {
                        found = vm.flatCategories[i];
                        break;
                    }
                }
                if (!found) break;
                names.unshift(found.categoryNameAr);
                id = found.parentCategoryID;
                guard += 1;
            }
            return names.join(' / ');
        }
    }
})();

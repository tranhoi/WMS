# 20240905_Shuei_VMS

SecretKey_MD5: WmsHt123@456

AUTORIZATION theo POLICY: 
- Có 3 roles:
	+ Admin: toàn quyền, vào tất cả các functions
	+ Staff: trừ group "Master Management" và "System Management"
	+ System: chỉ vào được các functions trong group "System Management"
- Policies: 
	+ Admin: role Admin
	+ Staff: role Staff
	+ System: role System
	+ AdminAndStaft: gồm 2 roles Admin + Staff. Dùng cho các page mà staff được vào thì kèm thêm Admin, như receipt, shipment,...
	+ AdminAndSystem: gồm 2 roles Admin + System. Dùng cho các page system được vào thì kèm thêm Admin

FrontEnd thêm dòng này vào trong file .razor
- @attribute [Authorize(Policy = "AdminAndStaff")]
- Thay 'AdminAndStaff' thành policy tương ứng

Update data type của cột status tất cả các tables, chuyển từ nvarchar --> int để dùng enum cho tiện
- Enum chuyển sang Domain.Enums
- Bỏ field Status ra khỏi modeel GenericEntity. Status của entity nào sẽ đưuọc quy định riêng theo Enum tương ứng
- Có các Enum sau:
	+ EnumStatus: dùng chung cho hầu hết Field Status của tất cả các entity chỉ trừ 1 số entity về receipt, warehouseTran
	+ EnumReceiptStatus: Dùng cho field Status của entity WarehouseReceiptOrder
	+ EnumProductStatus, EnumProductType: dùng cho product detail
	+ EnumWarehouseTransType: dùng cho field Trantype --> entity  WarehouseTran
	+ EnumStatusReceipt: dùng cho field StatusReceipt --> entity  WarehouseTran
	+ EnumStatusIssue: dùng cho field StatusIssue -- > entity  WarehouseTran

Với các respone trả về mọi người xử lý theo cơ chế sau cho gọn
extention a đã custome để nó trả về model 
Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
Trong API sẽ trả về cái result trên, Client chỉ cần check Succeeded là biết ddc API thực hiện thành công hay ko

Client check respone như sau cho gọn:

 var res = await _locationServices.GetByIdAsync(Guid.Parse(_id));
 if (!res.Succeeded)
 {
     _notificationService.Notify(new NotificationMessage
     {
         Severity = NotificationSeverity.Error,
         Summary = "Error",
         Detail = resMessage,
         Duration = 5000
     });
     return;
 }

//trả về data model
 _model = res.Data;

//trả về data List<model>
_model	= res.Data.ToList();

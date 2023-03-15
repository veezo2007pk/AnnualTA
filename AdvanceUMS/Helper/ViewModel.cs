using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdvanceUMS.Helper
{
    public class ViewModel
    {
    }

    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Invalid Username or Password Address")]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class MenuViewModel
    {
        public Nullable<int> ID { get; set; }
        public Nullable<int> ParentModuleID { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string ModuleName { get; set; }
        public string PageIcon { get; set; }
        public string PageURL { get; set; }
        public string PageSlug { get; set; }
        public List<SubMenuViewModel> SubMenu { get; set; }
    }

    public class SubMenuViewModel
    {
        public Nullable<int> ID { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string ModuleName { get; set; }
        public string PageIcon { get; set; }
        public string PageURL { get; set; }
        public string PageSlug { get; set; }
    }

    public class UserViewModel
    {
        public Nullable<Guid> ID { get; set; }
      
        public string FirstName { get; set; }

        public string LastName { get; set; }
       
        public string BirthDate { get; set; }
   
        public string Gender { get; set; }
       
        public Nullable<int> CountryID { get; set; }
        public Nullable<bool> IsSupervisor { get; set; }
        public Nullable<System.Guid> uqSupervisorCode { get; set; }
        public Nullable<bool> IsEngineer { get; set; }
        public Nullable<System.Guid> uqEngineerCode { get; set; }
        public Nullable<bool> IsHOD { get; set; }
        public Nullable<System.Guid> uqHODCode { get; set; }
        public string Phone { get; set; }
        public Nullable<System.Guid> uqContractorCode { get; set; }
       
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string cPassword { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<Guid> RoleID { get; set; }
        public string Address { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string GoogleLink { get; set; }
        public string LinkedInLink { get; set; }
        public string SkypeID { get; set; }
    }
    public  class tblOvertimeViewModel
    {
        public string uqOvertimeCode { get; set; }
        public Nullable<System.Guid> uqAttendanceCode { get; set; }
        public Nullable<int> intRecommendedHrs { get; set; }
        public Nullable<bool> bolIsVerified { get; set; }
        public Nullable<System.Guid> uqShiftCode { get; set; }
        public string Remarks { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
    }

    public class RoleViewModel
    {
        public Nullable<Guid> ID { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class ActivityViewModel
    {
        public Nullable<Guid> UserID { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public Nullable<DateTime> LogTime { get; set; }
        public string IPAddress { get; set; }
    }



    public partial class AccomodationViewModel
    {
        public Nullable<Guid> uqAccommodationCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strAccommodationName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }

    
    }
    public partial class ShiftViewModel
    {
        public Nullable<Guid> uqShiftId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strShiftName { get; set; }
        public Nullable<System.TimeSpan> tmFrom { get; set; }
        public Nullable<System.TimeSpan> tmTo { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class SkillViewModel
    {
        public Nullable<Guid> uqSkillCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strSkillName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class TradeViewModel
    {
        public Nullable<Guid> uqTradeCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strTradeName { get; set; }
        public Nullable<int> intRate { get; set; }
        public Nullable<int> intMealAllowance { get; set; }
        public Nullable<int> intOvertimeRate { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class SupervisorViewModel
    {
        public System.Guid? uqSupervisorId { get; set; }
        public string strSupervisorName { get; set; }
        public Nullable<System.Guid> uqEngineerCode { get; set; }
        public Nullable<System.Guid> uqDepartmentCode { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
    }


    public partial class HODViewModel
    {
        public System.Guid? uqHODId { get; set; }
        public string strHODName { get; set; }
        public Nullable<System.Guid> uqDepartmentCode { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
    }
    public partial class EngineerViewModel
    {
        public System.Guid? uqEngineerId { get; set; }
        public string strEngineerName { get; set; }
        public Nullable<System.Guid> uqDepartmentCode { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
        public Nullable<System.Guid> uqHODId { get; set; }
    }

    public partial class ContractorViewModel
    {
        public Nullable<Guid> uqContractorCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strContractorName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class ResidenceViewModel
    {
        public Nullable<Guid> uqResidenceCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strResidenceName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class DepartmentViewModel
    {
        public Nullable<Guid> uqDepartmentCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strDepartmentName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class ExperienceViewModel
    {
        public Nullable<Guid> uqExperienceCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strExperienceName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class AreaViewModel
    {
        public Nullable<Guid> uqAreaCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strAreaName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }


    public partial class JoiningStatViewModel
    {
        public Nullable<Guid> uqJoiningStatusCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strJoiningStatusCode { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class JoiningStatusViewModel
    {
        public Nullable<Guid> uqJoiningStatusCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strJoiningStatusName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }


    }
    public partial class WorkerViewModel
    {
        public string Year { get; set; }
        public System.Guid? uqAttendanceCode { get; set; }
        public int intRecommendedHrs { get; set; }
        public DateTime dtCheckIn { get; set; }
        public System.Guid? uqId { get; set; }
        public Nullable<System.Guid> uqContractorUserId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strName { get; set; }
        
        public string strFatherName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strNextOfKin { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strNextOfKinContactNo { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqRelationshipCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No must follow the XXXXX-XXXXXXX-X format!")]

        public string strCNIC { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strContact { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strEmergencyNo { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<int> intAge { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strWorkderId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strAddress { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqTradeCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string strQualification { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqSkillCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string uqExperienceCode { get; set; }
        
        public string strTAattended { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
       
        public Nullable<System.DateTime> dtLastTAyear { get; set; }
        
        public Nullable<decimal> intTARate { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqSupervisorCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqAreaCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> dtDurationFrom { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> dtDurationTo { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqResidenceCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqContractorCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqDepartmentCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqAccommodationCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> uqShiftCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> dtTAyear { get; set; }
        public byte[] vbrPicture { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<System.Guid> intJoiningStatusCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public Nullable<int> intTACurrentRate { get; set; }
        public Nullable<System.DateTime> dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public Nullable<System.Guid> uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
    }
}
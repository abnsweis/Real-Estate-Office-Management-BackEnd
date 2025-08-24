using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Enums;


public enum enApiErrorCode {

     
      

    // --- 0 - 999: General / System Errors --- 
    ServiceUnavailable = 1,          
    ConfigurationError = 2,          
    GeneralError = 3,        
    InvalidEnumValue = 4,
    ServerFileCopyFailure = 5,
    NotFoundDefaultImagePath = 6,
    Unknown = 7,
    InvalidGuid = 8,
    RequiredGreaterThanZero = 9,
    UnexpectedEntityCreationFailure = 10,
    Forbidden = 11,

    // --- 1000 - 1499 :  User & Authentication Errors --- 
    UserNotFound = 1001,             
    EmailAlreadyTaken = 1002,        
    InvalidCredentials = 1003,       
    PasswordTooWeak = 1004,          
    UserAccountLocked = 1005,        
    TokenInvalidOrExpired = 1006,
    Unauthorized = 1007,        
    UsernameAlreadyTaken = 1008,     
    PhoneAlreadyTaken = 1009,
    UserDeactivated = 1010,        

    // --- 1500 - 1999 : Identity  Errors ---
    UnknownError = 1500,
    ConcurrencyError = 1501,
    InvalidPassword = 1502,
    InvalidToken = 1503,
    InvalidRecoveryCode = 1504,
    DuplicateLogin = 1505,
    InvalidUserName = 1506,
    InvalidEmail = 1507,
    DuplicateUserName = 1508,
    DuplicateEmail = 1509,
    InvalidRoleName = 1500,
    DuplicateRoleName = 1511,
    LockoutNotEnabled = 1512,
    UserAlreadyInRole = 1513,
    UserNotInRole = 1514,
    PasswordTooShort = 1515,
    PasswordRequiresUniqueChars = 1516,
    PasswordRequiresNonAlphanumeric = 1517,
    PasswordRequiresLower = 1518,
    PasswordRequiresUpper = 1519,
    PasswordRequiresDigit = 1520,
    UserAlreadyHasPassword = 1521,



    // --- 2000 - 2499 : Files  Errors ---

    InvalidFileExtension = 2000,
    MissingUploadedFile = 2001,
    FileSaveError = 2002,
    FileDeleteError = 2003,
    UnsupportedFileType = 2004,
    ServerConfigurationError = 2005,
    MissingFileName = 2006,

    // --- 5000 - 5999 : General Validation Errors ---
    GeneralValidation = 5000,           
    RequiredField = 5001,               
    InvalidFormat = 5002,               
    MinimumLengthViolated = 5003,       
    MaximumLengthExceeded = 5004,
    MinimumAgeViolated = 5005,
    MaximumAgeViolated = 5006,
    PasswordMismatch = 5007,            
    InvalidDate = 5008,          








    // --- 2500 - 2999 : Customers  Errors ---
    CustomerNotFound = 2500,              
    NationallDAlreadyTaken = 2501,        
    InValidCustomerType = 2502,           
    DuplicateCustomer = 2503,
    MissingNationallD = 2504,
    // --- 3000 - 3499 : People  Errors ---
    InValidGender = 3000,



    // --- 3500 - 3999 : Categories  Errors ---
    CategoryNotFound = 3500,
    CategoryNameAlreadyExists = 3501,
    MissingCategoryName = 3502,
    InvalidCategoryNameLength = 3503,

    // --- 4000 - 4499 : Properties  Errors ---
    PropertyNotFound = 4000,
    NotAvailable = 4001,
    SellerNotOwner = 4003,

    // --- 4500 - 4999 : Sales  Errors ---
    SaleNotFound = 4501,
    SellerAndBuyerCannotBeSame = 4502,
    CreationFailed = 4503,

    // --- 5000 - 5499 : Rentals  Errors ---
    RentalNotFound = 5000, 
    LessorNotOwner = 5002,

    // --- 5500 - 5999 : Testimonials  Errors ---
    TestimonialAlreadyExists = 5501,

    // --- 6000 - 6499 : comments  Errors ---
    CommentNotFound = 6000,

    // --- 6500 - 6999 : favorites  Errors ---
    FavoriteNotFound = 6501,
    sssssssssssssss = 6501, 
}

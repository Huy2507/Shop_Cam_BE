namespace Shop_Cam_BE.Application.Common.Constants;

public static class ErrorCodes
{
    public const string EMAIL_ALREADY_EXISTS = "EMAIL_ALREADY_EXISTS";
    public const string USERNAME_ALREADY_EXISTS = "USERNAME_ALREADY_EXISTS";
    public const string INVALID_EMAIL = "INVALID_EMAIL";
    public const string INVALID_PASSWORD = "INVALID_PASSWORD";
    public const string PASSWORD_MISMATCH = "PASSWORD_MISMATCH";
    public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
    public const string INVALID_OTP = "INVALID_OTP";
    public const string OTP_EXPIRED = "OTP_EXPIRED";
    public const string OTP_SEND_FAILED = "OTP_SEND_FAILED";
    public const string USER_NOT_FOUND = "USER_NOT_FOUND";
    public const string ACCOUNT_LOCKED = "ACCOUNT_LOCKED";
    public const string TOKEN_EXPIRED = "TOKEN_EXPIRED";
    public const string TOKEN_INVALID = "TOKEN_INVALID";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string UNAUTHORIZED_ROLE = "UNAUTHORIZED_ROLE";
    public const string INVALID_REFRESH_TOKEN = "INVALID_REFRESH_TOKEN";
    public const string REFRESH_TOKEN_FAILED = "REFRESH_TOKEN_FAILED";
    public const string REGISTRATION_FAILED = "REGISTRATION_FAILED";
    public const string TOO_MANY_ATTEMPTS = "TOO_MANY_ATTEMPTS";
    public const string LOGOUT_FAILED = "LOGOUT_FAILED";
    public const string EMAIL_NOT_FOUND = "EMAIL_NOT_FOUND";
    public const string INVALID_USERNAME = "INVALID_USERNAME";
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string NOT_FOUND = "NOT_FOUND";
    public const string SERVER_ERROR = "SERVER_ERROR";
    public const string OPERATION_FAILED = "OPERATION_FAILED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";
    public const string INVALID_DATA = "INVALID_DATA";

    // Mã lỗi nghiệp vụ bổ sung — FE dịch qua i18n; API không trả chuỗi hiển thị cho người dùng.
    public const string PRODUCT_DELETE_CONFLICT_ORDERS = "PRODUCT_DELETE_CONFLICT_ORDERS";
    public const string CATEGORY_DELETE_HAS_PRODUCTS = "CATEGORY_DELETE_HAS_PRODUCTS";
    public const string MISSING_QUERY_PARAMETER = "MISSING_QUERY_PARAMETER";
    public const string EMPTY_SITE_SETTINGS_ITEMS = "EMPTY_SITE_SETTINGS_ITEMS";
    public const string SITE_SETTING_KEY_NOT_ALLOWED = "SITE_SETTING_KEY_NOT_ALLOWED";
    public const string RESEND_OTP_NOT_SUPPORTED = "RESEND_OTP_NOT_SUPPORTED";
    public const string OTP_RESEND_COOLDOWN = "OTP_RESEND_COOLDOWN";
    public const string ORDER_REQUIRES_ITEMS = "ORDER_REQUIRES_ITEMS";
    public const string ORDER_PRODUCTS_NOT_FOUND = "ORDER_PRODUCTS_NOT_FOUND";
    public const string MISSING_USERNAME_OR_EMAIL = "MISSING_USERNAME_OR_EMAIL";
}

using System.Security.Claims;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Entities;

namespace HiringPipelineAPI.Middleware
{
    /// <summary>
    /// Middleware to capture HTTP request information for audit logging
    /// </summary>
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // Skip audit logging for certain paths
            if (ShouldSkipAudit(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var user = context.User;
            var userId = GetUserId(user);
            var username = GetUsername(user);
            var userRole = GetUserRole(user);
            var ipAddress = GetClientIpAddress(context);
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

            // Log the request
            if (userId.HasValue && !string.IsNullOrEmpty(username))
            {
                try
                {
                    var action = GetActionFromRequest(context.Request);
                    var entity = GetEntityFromPath(context.Request.Path);
                    
                    // Only log if we have a meaningful action (not just "View")
                    if (action != "View")
                    {
                        // Check if this is a supporting API call (not primary navigation)
                        if (IsSupportingApiCall(context.Request, action))
                        {
                            // Log supporting calls as BackgroundFetch instead of skipping them
                            // This helps distinguish between direct navigation and background data fetches
                            _logger.LogDebug("Logging supporting API call as BackgroundFetch: {Path} from referer: {Referer}", 
                                context.Request.Path, context.Request.Headers["Referer"].FirstOrDefault());
                            
                            await auditService.LogAsync(
                                userId.Value,
                                username,
                                userRole,
                                action,
                                entity,
                                GetEntityIdFromPath(context.Request.Path),
                                null, // changes
                                $"Background fetch: HTTP {context.Request.Method} {context.Request.Path}", // details
                                ipAddress,
                                userAgent,
                                AuditLogType.BackgroundFetch
                            );
                        }
                        else
                        {
                            // Determine log type based on context
                            var logType = DetermineLogType(context.Request, action);
                            
                            // Log primary navigation actions
                            await auditService.LogAsync(
                                userId.Value,
                                username,
                                userRole,
                                action,
                                entity,
                                GetEntityIdFromPath(context.Request.Path),
                                null, // changes
                                $"HTTP {context.Request.Method} {context.Request.Path}", // details
                                ipAddress,
                                userAgent,
                                logType
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to log audit entry for request {Path}", context.Request.Path);
                }
            }

            await _next(context);
        }

        private bool ShouldSkipAudit(PathString path)
        {
            var skipPaths = new[]
            {
                "/api/auditlogs", // Don't audit audit log requests
                "/api/auth/login", // Don't audit login attempts (handled separately)
                "/api/auth/refresh", // Don't audit token refresh
                "/api/auth/logout", // Don't audit logout (handled separately)
                "/api/auth/revoke", // Don't audit token revocation
                "/api/users/debug", // Don't audit debug endpoints
                "/api/analytics", // Don't audit analytics calls
                "/api/fileupload", // Don't audit file upload calls
                "/swagger",
                "/health",
                "/favicon.ico"
            };

            return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath));
        }

        private int? GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }

        private string GetUsername(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
        }

        private string GetUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
        }

        private string GetClientIpAddress(HttpContext context)
        {
            try
        {
            // Check for forwarded IP first (for load balancers/proxies)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                    var ip = forwardedFor.Split(',')[0].Trim();
                    if (IsValidIpAddress(ip))
                    {
                        _logger.LogDebug("Using X-Forwarded-For IP: {IP}", ip);
                        return ip;
                    }
                }

                // Check X-Real-IP header
                var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp) && IsValidIpAddress(realIp))
                {
                    _logger.LogDebug("Using X-Real-IP: {IP}", realIp);
                    return realIp;
                }

                // Check CF-Connecting-IP (Cloudflare)
                var cfIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(cfIp) && IsValidIpAddress(cfIp))
                {
                    _logger.LogDebug("Using CF-Connecting-IP: {IP}", cfIp);
                    return cfIp;
                }

                // Get the remote IP address
                var remoteIp = context.Connection.RemoteIpAddress;
                if (remoteIp != null)
                {
                    // Handle IPv4-mapped IPv6 addresses
                    if (remoteIp.IsIPv4MappedToIPv6)
                    {
                        remoteIp = remoteIp.MapToIPv4();
                    }
                    var ipString = remoteIp.ToString();
                    _logger.LogDebug("Using RemoteIpAddress: {IP}", ipString);
                    return ipString;
                }

                // Fallback to localhost for development
                _logger.LogDebug("Using fallback IP: 127.0.0.1");
                return "127.0.0.1";
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error getting client IP address: {Error}", ex.Message);
                return "127.0.0.1";
            }
        }

        private bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            // Check for common invalid IPs (but allow localhost for development)
            if (ip == "0.0.0.0" || ip == "unknown")
                return false;

            // Allow localhost addresses for development
            if (ip == "::1" || ip == "127.0.0.1" || ip == "localhost")
                return true;

            return System.Net.IPAddress.TryParse(ip, out _);
        }

        private bool IsSupportingApiCall(HttpRequest request, string action)
        {
            // This method determines if an API call is a supporting call (not primary navigation)
            // Supporting calls are those made to load related data for display purposes
            
            var path = request.Path.Value?.ToLower() ?? "";
            var referer = request.Headers["Referer"].FirstOrDefault() ?? "";
            
            // Debug logging to help troubleshoot
            _logger.LogDebug("IsSupportingApiCall - Path: {Path}, Referer: {Referer}, Action: {Action}", 
                path, referer, action);
            
            // Check for custom header that indicates this is a dashboard background call
            var isDashboardCall = request.Headers["X-Dashboard-Background"].FirstOrDefault() == "true";
            if (isDashboardCall)
            {
                _logger.LogDebug("Detected dashboard background call via custom header");
                return true;
            }
            
            // If there's no referer, it's likely a direct navigation (primary)
            if (string.IsNullOrEmpty(referer))
                return false;
            
            // Check if this is a supporting call based on the referer and current path
            if (referer?.Contains("/applications") == true)
            {
                // When on applications page, candidates and requisitions calls are supporting
                return path == "/api/candidates" || path == "/api/requisitions" || path == "/api/stagehistory";
            }
            
            if (referer?.Contains("/candidates") == true)
            {
                // When on candidates page, applications and requisitions calls are supporting
                return path == "/api/applications" || path == "/api/requisitions" || path == "/api/stagehistory";
            }
            
            if (referer?.Contains("/requisitions") == true)
            {
                // When on requisitions page, applications and candidates calls are supporting
                return path == "/api/applications" || path == "/api/candidates" || path == "/api/stagehistory";
            }
            
            if (referer?.Contains("/dashboard") == true)
            {
                // When on dashboard page, ALL entity calls are supporting (dashboard loads all data for charts/analytics)
                _logger.LogDebug("Detected dashboard referer, treating {Path} as supporting call", path);
                return path == "/api/applications" || path == "/api/candidates" || 
                       path == "/api/requisitions" || path == "/api/users" || 
                       path == "/api/stagehistory";
            }
            
            if (referer?.Contains("/stage-history") == true)
            {
                // When on stage history page, all entity calls are supporting (stage history loads all data)
                return path == "/api/applications" || path == "/api/candidates" || 
                       path == "/api/requisitions" || path == "/api/users";
            }
            
            // If we can't determine the context, log it (better to log than miss)
            return false;
        }

        private string GetActionFromRequest(HttpRequest request)
        {
            // Get more specific action based on the path
            var path = request.Path.Value?.ToLower() ?? "";
            
            if (request.Method == "GET")
            {
                // Log main entity views
                if (path == "/api/users")
                    return "View Users";
                else if (path == "/api/candidates")
                    return "View Candidates";
                else if (path == "/api/applications")
                    return "View Applications";
                else if (path == "/api/requisitions")
                    return "View Requisitions";
                else if (path == "/api/stagehistory")
                    return "View Stage History";
                else if (path == "/api/roles")
                    return "View Roles";
                else if (path == "/api/auditlogs")
                    return "View Audit Logs";
                else
                    return "View";
            }
            
            // Handle specific actions for different entities
            if (path.StartsWith("/api/stagehistory"))
            {
                return request.Method switch
                {
                    "POST" => "Create Stage History Entry",
                    "PUT" => "Update Stage History Entry", 
                    "PATCH" => "Update Stage History Entry",
                    "DELETE" => "Delete Stage History Entry",
                    _ => request.Method
                };
            }
            
            // Handle user management specific actions
            if (path.StartsWith("/api/users"))
            {
                if (path.Contains("/change-password"))
                    return "Change Password";
                else if (path.Contains("/deactivate"))
                    return "Deactivate User";
                else if (path.Contains("/activate"))
                    return "Activate User";
                else if (path.Contains("/roles") && request.Method == "POST")
                    return "Assign Role";
                else if (path.Contains("/roles") && request.Method == "DELETE")
                    return "Remove Role";
                else
                    return request.Method switch
                    {
                        "POST" => "Create User",
                        "PUT" => "Update User",
                        "PATCH" => "Update User",
                        "DELETE" => "Delete User",
                        _ => request.Method
                    };
            }
            
            // Handle requisition specific actions
            if (path.StartsWith("/api/requisitions"))
            {
                if (path.Contains("/publish"))
                    return "Publish Requisition";
                else if (path.Contains("/close"))
                    return "Close Requisition";
                else
                    return request.Method switch
                    {
                        "POST" => "Create Requisition",
                        "PUT" => "Update Requisition",
                        "PATCH" => "Update Requisition",
                        "DELETE" => "Delete Requisition",
                        _ => request.Method
                    };
            }
            
            // Handle candidate specific actions
            if (path.StartsWith("/api/candidates"))
            {
                if (path.Contains("/upload-resume"))
                    return "Upload Resume";
                else if (path.Contains("/add-skills"))
                    return "Add Skills";
                else if (path.Contains("/archive"))
                    return "Archive Candidate";
                else
                    return request.Method switch
                    {
                        "POST" => "Create Candidate",
                        "PUT" => "Update Candidate",
                        "PATCH" => "Update Candidate",
                        "DELETE" => "Delete Candidate",
                        _ => request.Method
                    };
            }
            
            // Handle application specific actions
            if (path.StartsWith("/api/applications"))
            {
                if (path.Contains("/change-status"))
                    return "Change Application Status";
                else if (path.Contains("/move-stage"))
                    return "Move Candidate to Stage";
                else
                    return request.Method switch
                    {
                        "POST" => "Create Application",
                        "PUT" => "Update Application",
                        "PATCH" => "Update Application",
                        "DELETE" => "Delete Application",
                        _ => request.Method
                    };
            }
            
            // Handle database management actions
            if (path.StartsWith("/api/database"))
            {
                if (path.Contains("/clear-selected"))
                    return "Clear Selected Tables";
                else if (path.Contains("/backup"))
                    return "Database Backup";
                else if (path.Contains("/restore"))
                    return "Database Restore";
                else if (path.Contains("/reset"))
                    return "Database Reset";
                else if (path.Contains("/export"))
                    return "Export Data";
                else
                    return "Database Management";
            }
            
            // Handle audit logs specific actions
            if (path.StartsWith("/api/auditlogs"))
            {
                if (path.Contains("/export"))
                    return "Export Audit Logs";
                else if (path.Contains("/clear"))
                    return "Clear Audit Logs";
                else if (path.Contains("/delete"))
                    return "Delete Audit Entry";
                else
                    return "Audit Logs Management";
            }
            
            return request.Method switch
            {
                "POST" => "Create",
                "PUT" => "Update",
                "PATCH" => "Update",
                "DELETE" => "Delete",
                _ => request.Method
            };
        }

        private string GetEntityFromPath(PathString path)
        {
            var pathValue = path.Value ?? "";
            var segments = pathValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0] == "api")
            {
                var entity = segments[1];
                // Handle specific entity mappings
                return entity.ToLower() switch
                {
                    "users" => "Users",
                    "candidates" => "Candidates", 
                    "applications" => "Applications",
                    "requisitions" => "Requisitions",
                    "stagehistory" => "Stage History",
                    "stagehistories" => "Stage History",
                    "roles" => "Roles",
                    "permissions" => "Permissions",
                    "auditlogs" => "Audit Logs",
                    _ => entity
                };
            }
            return "Unknown";
        }

        private int? GetEntityIdFromPath(PathString path)
        {
            var pathValue = path.Value ?? "";
            var segments = pathValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 3 && segments[0] == "api")
            {
                if (int.TryParse(segments[2], out var entityId))
                {
                    return entityId;
                }
            }
            return null;
        }

        private AuditLogType DetermineLogType(HttpRequest request, string action)
        {
            var path = request.Path.Value?.ToLower() ?? "";
            var referer = request.Headers["Referer"].FirstOrDefault() ?? "";
            
            // Authentication actions
            if (path.StartsWith("/api/auth/"))
            {
                return AuditLogType.Authentication;
            }
            
            // Database management actions
            if (path.StartsWith("/api/database/"))
            {
                return AuditLogType.DatabaseManagement;
            }
            
            // Check if this is a background fetch (GET requests with referer)
            if (request.Method == "GET" && !string.IsNullOrEmpty(referer))
            {
                // Check if this is a supporting API call (background fetch)
                if (IsSupportingApiCall(request, action))
                {
                    return AuditLogType.BackgroundFetch;
                }
            }
            
            // Check if this is a system operation
            if (IsSystemOperation(action, path))
            {
                return AuditLogType.SystemOperation;
            }
            
            // Default to user action
            return AuditLogType.UserAction;
        }

        private bool IsBackgroundDataFetch(string path, string action)
        {
            // These are typically background fetches when loading pages
            var backgroundFetchPatterns = new[]
            {
                "/api/applications",
                "/api/candidates", 
                "/api/requisitions",
                "/api/stagehistory",
                "/api/users"
            };
            
            return backgroundFetchPatterns.Any(pattern => path.StartsWith(pattern)) && action == "View";
        }

        private bool IsSystemOperation(string action, string path)
        {
            // These are typically system operations
            var systemOperations = new[]
            {
                "System",
                "Background",
                "Scheduled",
                "Automatic"
            };
            
            return systemOperations.Any(op => action.Contains(op)) || 
                   path.Contains("/system/") || 
                   path.Contains("/background/");
        }
    }
}

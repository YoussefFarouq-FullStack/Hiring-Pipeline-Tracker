// Custom Swagger UI JavaScript Enhancements

(function() {
    'use strict';

    // Wait for Swagger UI to be fully loaded
    function waitForSwaggerUI() {
        if (typeof SwaggerUIBundle !== 'undefined') {
            initializeCustomFeatures();
        } else {
            setTimeout(waitForSwaggerUI, 100);
        }
    }

    function initializeCustomFeatures() {
        console.log('Initializing custom Swagger UI features...');
        
        // Add request duration tracking
        addRequestDurationTracking();
        
        // Add response time estimation
        addResponseTimeEstimation();
        
        // Add keyboard shortcuts
        addKeyboardShortcuts();
        
        // Add better error handling
        enhanceErrorHandling();
        
        // Add request/response logging
        addRequestResponseLogging();
        
        // Add sample data generation
        addSampleDataGeneration();
        
        // Add operation search highlighting
        addOperationSearchHighlighting();
    }

    function addRequestDurationTracking() {
        // Track request duration for better UX
        const originalFetch = window.fetch;
        window.fetch = function(...args) {
            const startTime = performance.now();
            return originalFetch.apply(this, args).then(response => {
                const duration = performance.now() - startTime;
                console.log(`Request completed in ${duration.toFixed(2)}ms`);
                return response;
            });
        };
    }

    function addResponseTimeEstimation() {
        // Add estimated response time to operation blocks
        const operationBlocks = document.querySelectorAll('.opblock');
        operationBlocks.forEach(block => {
            const method = block.querySelector('.opblock-summary-method');
            if (method) {
                const estimatedTime = document.createElement('div');
                estimatedTime.className = 'estimated-response-time';
                estimatedTime.style.cssText = 'font-size: 12px; color: #7f8c8d; margin-top: 4px;';
                estimatedTime.textContent = 'Estimated response time: 100-500ms';
                method.appendChild(estimatedTime);
            }
        });
    }

    function addKeyboardShortcuts() {
        // Add keyboard shortcuts for common actions
        document.addEventListener('keydown', function(e) {
            // Ctrl/Cmd + Enter to execute current operation
            if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
                const executeBtn = document.querySelector('.execute-wrapper .btn.execute');
                if (executeBtn && !executeBtn.disabled) {
                    executeBtn.click();
                }
            }
            
            // Ctrl/Cmd + K to focus search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                const searchInput = document.querySelector('.filter input');
                if (searchInput) {
                    searchInput.focus();
                }
            }
            
            // Escape to close modals or expandable sections
            if (e.key === 'Escape') {
                const expandedSections = document.querySelectorAll('.opblock.is-open');
                expandedSections.forEach(section => {
                    const toggleBtn = section.querySelector('.opblock-summary');
                    if (toggleBtn) {
                        toggleBtn.click();
                    }
                });
            }
        });
    }

    function enhanceErrorHandling() {
        // Enhance error messages with more context
        const originalConsoleError = console.error;
        console.error = function(...args) {
            if (args[0] && typeof args[0] === 'string' && args[0].includes('Swagger')) {
                console.log('Swagger UI Error:', ...args);
                // Add user-friendly error messages
                showUserFriendlyError(args[0]);
            }
            originalConsoleError.apply(console, args);
        };
    }

    function showUserFriendlyError(errorMessage) {
        // Create a user-friendly error notification
        const notification = document.createElement('div');
        notification.className = 'swagger-error-notification';
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #e74c3c;
            color: white;
            padding: 16px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 10000;
            max-width: 400px;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        `;
        
        notification.innerHTML = `
            <div style="font-weight: 600; margin-bottom: 8px;">API Error</div>
            <div style="font-size: 14px;">${errorMessage}</div>
            <button onclick="this.parentElement.remove()" style="
                background: rgba(255,255,255,0.2);
                border: none;
                color: white;
                padding: 4px 8px;
                border-radius: 4px;
                margin-top: 8px;
                cursor: pointer;
            ">Dismiss</button>
        `;
        
        document.body.appendChild(notification);
        
        // Auto-remove after 10 seconds
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 10000);
    }

    function addRequestResponseLogging() {
        // Add a logging panel for requests and responses
        const loggingPanel = document.createElement('div');
        loggingPanel.id = 'swagger-logging-panel';
        loggingPanel.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            width: 400px;
            max-height: 300px;
            background: white;
            border: 1px solid #ddd;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 10000;
            font-family: monospace;
            font-size: 12px;
        `;
        
        loggingPanel.innerHTML = `
            <div style="
                background: #f8f9fa;
                padding: 12px;
                border-bottom: 1px solid #ddd;
                border-radius: 8px 8px 0 0;
                font-weight: 600;
                display: flex;
                justify-content: space-between;
                align-items: center;
            ">
                <span>API Request Log</span>
                <button onclick="clearLog()" style="
                    background: #dc3545;
                    color: white;
                    border: none;
                    padding: 4px 8px;
                    border-radius: 4px;
                    cursor: pointer;
                    font-size: 11px;
                ">Clear</button>
            </div>
            <div id="log-content" style="
                padding: 12px;
                max-height: 250px;
                overflow-y: auto;
                background: #f8f9fa;
            "></div>
        `;
        
        document.body.appendChild(loggingPanel);
        
        // Hide by default, show on hover
        loggingPanel.style.opacity = '0.1';
        loggingPanel.addEventListener('mouseenter', () => {
            loggingPanel.style.opacity = '1';
        });
        loggingPanel.addEventListener('mouseleave', () => {
            loggingPanel.style.opacity = '0.1';
        });
    }

    function addSampleDataGeneration() {
        // Add sample data generation for request bodies
        const requestBodies = document.querySelectorAll('.body-param__text');
        requestBodies.forEach(body => {
            const generateBtn = document.createElement('button');
            generateBtn.textContent = 'Generate Sample';
            generateBtn.className = 'generate-sample-btn';
            generateBtn.style.cssText = `
                background: #28a745;
                color: white;
                border: none;
                padding: 4px 8px;
                border-radius: 4px;
                margin-left: 8px;
                cursor: pointer;
                font-size: 11px;
            `;
            
            generateBtn.addEventListener('click', () => {
                generateSampleData(body);
            });
            
            body.parentElement.appendChild(generateBtn);
        });
    }

    function generateSampleData(bodyElement) {
        // Simple sample data generation based on field names
        const textarea = bodyElement.querySelector('textarea');
        if (textarea) {
            const sampleData = {
                firstName: "John",
                lastName: "Doe",
                email: "john.doe@example.com",
                phone: "+1-555-0123",
                linkedInUrl: "https://linkedin.com/in/johndoe",
                source: "LinkedIn",
                status: "Applied"
            };
            
            textarea.value = JSON.stringify(sampleData, null, 2);
            textarea.dispatchEvent(new Event('input', { bubbles: true }));
        }
    }

    function addOperationSearchHighlighting() {
        // Highlight search terms in operation descriptions
        const searchInput = document.querySelector('.filter input');
        if (searchInput) {
            searchInput.addEventListener('input', function(e) {
                const searchTerm = e.target.value.toLowerCase();
                const operations = document.querySelectorAll('.opblock-summary-description');
                
                operations.forEach(operation => {
                    const text = operation.textContent;
                    if (searchTerm && text.toLowerCase().includes(searchTerm)) {
                        operation.style.backgroundColor = '#fff3cd';
                        operation.style.padding = '4px 8px';
                        operation.style.borderRadius = '4px';
                    } else {
                        operation.style.backgroundColor = '';
                        operation.style.padding = '';
                        operation.style.borderRadius = '';
                    }
                });
            });
        }
    }

    // Global functions for the logging panel
    window.clearLog = function() {
        const logContent = document.getElementById('log-content');
        if (logContent) {
            logContent.innerHTML = '';
        }
    };

    window.addLogEntry = function(entry) {
        const logContent = document.getElementById('log-content');
        if (logContent) {
            const logEntry = document.createElement('div');
            logEntry.style.cssText = 'margin-bottom: 8px; padding: 4px; border-left: 3px solid #007bff;';
            logEntry.textContent = `[${new Date().toLocaleTimeString()}] ${entry}`;
            logContent.appendChild(logEntry);
            logContent.scrollTop = logContent.scrollHeight;
        }
    };

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', waitForSwaggerUI);
    } else {
        waitForSwaggerUI();
    }

})();

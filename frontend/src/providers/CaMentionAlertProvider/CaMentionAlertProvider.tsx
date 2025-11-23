import {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useState,
    type ReactNode,
} from "react";
import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
} from "@microsoft/signalr";
import type { CaMentionAlert } from "../../models/Alert";
import { API_ENDPOINTS, SIGNALR_HUBS } from "../../config/api";

interface CaMentionAlertContextType {
    alerts: CaMentionAlert[];
    isConnected: boolean;
    connectionError: string | null;
    clearAlerts: () => void;
    removeAlert: (alertId: string) => void;
}

const CaMentionAlertContext = createContext<CaMentionAlertContextType | undefined>(
    undefined,
);

interface CaMentionAlertProviderProps {
    children: ReactNode;
}

export const CaMentionAlertProvider: React.FC<CaMentionAlertProviderProps> = ({
    children,
}) => {
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [alerts, setAlerts] = useState<CaMentionAlert[]>([]);
    const [connectionError, setConnectionError] = useState<string | null>(null);

    const clearAlerts = () => setAlerts([]);

    const removeAlert = async (alertId: string) => {
        try {
            // Call the API to remove the alert
            const response = await fetch(API_ENDPOINTS.alerts.remove(alertId), {
                method: "DELETE",
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Remove from local state
            setAlerts((prev) => prev.filter(alert => alert.id !== alertId));
        } catch (error) {
            console.error("Failed to remove CA mention alert:", error);
            // Still remove from local state even if API call fails
            setAlerts((prev) => prev.filter(alert => alert.id !== alertId));
        }
    };

    // Helper function to convert numeric AlertType to string
    const convertAlertType = (type: number | string): 'TwitterCaMention' | null => {
        if (typeof type === 'string') return type === 'TwitterCaMention' ? type : null;
        return type === 3 ? 'TwitterCaMention' : null;
    };

    // Initialize alerts
    const initializeAlerts = useCallback(async () => {
        try {
            const response = await fetch(API_ENDPOINTS.alerts.caMentions, {
                method: "GET",
            });

            if (response.status === 404) {
                return;
            }

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const caMentionAlerts = await response.json();

            // Convert numeric types to strings if needed
            const convertedAlerts = caMentionAlerts
                .map((alert: any) => ({
                    ...alert,
                    type: convertAlertType(alert.type)
                }))
                .filter((alert: any) => alert.type !== null) as CaMentionAlert[];

            setAlerts(convertedAlerts);
        } catch (error) {
            console.error("Failed to initialize CA mention alerts:", error);
        }
    }, []);

    // Initialize alerts on mount
    useEffect(() => {
        initializeAlerts();
    }, [initializeAlerts]);

    // Setup hub connection
    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(SIGNALR_HUBS.caMentions)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, []);

    // Start SignalR connection
    useEffect(() => {
        if (!connection) return;

        const startConnection = async () => {
            try {
                await connection.start();
                setIsConnected(true);
                setConnectionError(null);
            } catch (error) {
                setConnectionError(
                    error instanceof Error ? error.message : "Connection failed",
                );
                console.error("CaMentionAlert SignalR connection failed:", error);
            }
        };

        startConnection();

        connection.onclose(() => {
            setIsConnected(false);
        });

        connection.onreconnected(() => {
            setIsConnected(true);
            setConnectionError(null);
        });

        connection.onreconnecting(() => {
            setIsConnected(false);
        });

        // Listen for AlertAdded events and filter for CA mention alerts
        connection.on("AlertAdded", (alert: any) => {
            const convertedType = convertAlertType(alert.type);
            if (convertedType === 'TwitterCaMention') {
                const caMentionAlert = { ...alert, type: convertedType } as CaMentionAlert;

                setAlerts((prev) => {
                    // For CaMention alerts, replace existing alert for the same coin address
                    const existingIndex = prev.findIndex(existingAlert =>
                        existingAlert.coinAddress === alert.coinAddress
                    );

                    if (existingIndex >= 0) {
                        // Replace existing alert
                        const newAlerts = [...prev];
                        newAlerts[existingIndex] = caMentionAlert;
                        return newAlerts;
                    }

                    // Add new alert to beginning
                    return [caMentionAlert, ...prev];
                });
            }
        });

        // Listen for AlertRemoved events
        connection.on("AlertRemoved", (alertId: string) => {
            setAlerts((prev) => prev.filter(alert => alert.id !== alertId));
        });

        return () => {
            connection.stop();
        };
    }, [connection]);

    const contextValue: CaMentionAlertContextType = {
        alerts,
        isConnected,
        connectionError,
        clearAlerts,
        removeAlert,
    };

    return (
        <CaMentionAlertContext.Provider value={contextValue}>
            {children}
        </CaMentionAlertContext.Provider>
    );
};

export const useCaMentionAlerts = () => {
    const context = useContext(CaMentionAlertContext);
    if (!context) {
        throw new Error("useCaMentionAlerts must be used within a CaMentionAlertProvider");
    }
    return context;
};

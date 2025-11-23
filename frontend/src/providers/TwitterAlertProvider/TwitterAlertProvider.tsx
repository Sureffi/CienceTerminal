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
import type { TwitterAlert } from "../../models/Alert";
import { API_ENDPOINTS, SIGNALR_HUBS } from "../../config/api";

interface TwitterAlertContextType {
    alerts: TwitterAlert[];
    isConnected: boolean;
    connectionError: string | null;
    clearAlerts: () => void;
    removeAlert: (alertId: string) => void;
}

const TwitterAlertContext = createContext<TwitterAlertContextType | undefined>(
    undefined,
);

interface TwitterAlertProviderProps {
    children: ReactNode;
}

export const TwitterAlertProvider: React.FC<TwitterAlertProviderProps> = ({
    children,
}) => {
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [alerts, setAlerts] = useState<TwitterAlert[]>([]);
    const [connectionError, setConnectionError] = useState<string | null>(null);

    const clearAlerts = () => setAlerts([]);

    const removeAlert = async (alertId: string) => {
        // Just remove from local state - backend handles alert lifecycle
        setAlerts((prev) => prev.filter(alert => alert.id !== alertId));
    };

    // Helper function to convert numeric AlertType to string
    const convertAlertType = (type: number | string): 'TwitterLegit' | 'TwitterSpam' | 'TwitterPreLaunch' | null => {
        if (typeof type === 'string') {
            return type === 'TwitterLegit' || type === 'TwitterSpam' || type === 'TwitterPreLaunch' ? type : null;
        }

        switch (type) {
            case 0: return 'TwitterLegit';
            case 1: return 'TwitterSpam';
            case 2: return 'TwitterPreLaunch';
            default: return null;
        }
    };

    // Helper function to check if alert is a Twitter alert (excluding CA mentions)
    const isTwitterAlert = (type: number | string): boolean => {
        const convertedType = convertAlertType(type);
        return convertedType !== null;
    };

    // Initialize alerts
    const initializeAlerts = useCallback(async () => {
        try {
            // Fetch without authentication
            const response = await fetch(API_ENDPOINTS.alerts.twitter);

            if (response.status === 404) {
                return;
            }

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const twitterAlerts = await response.json();

            // Convert numeric types to strings and filter out non-Twitter alerts
            const convertedAlerts = twitterAlerts
                .map((alert: any) => ({
                    ...alert,
                    type: convertAlertType(alert.type)
                }))
                .filter((alert: any) => alert.type !== null) as TwitterAlert[];

            setAlerts(convertedAlerts);
        } catch (error) {
            console.error("Failed to initialize Twitter alerts:", error);
        }
    }, []);

    // Initialize alerts (authentication disabled for now)
    useEffect(() => {
        // Skip authentication check - initialize immediately
        initializeAlerts();
    }, [initializeAlerts]);

    // Setup hub connection (authentication disabled)
    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(SIGNALR_HUBS.twitter)
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
                console.error("TwitterAlert SignalR connection failed:", error);
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

        // Listen for AlertAdded events and filter for TwitterAlerts (excluding CA mentions)
        connection.on("AlertAdded", (alert: any) => {
            if (isTwitterAlert(alert.type)) {
                const convertedType = convertAlertType(alert.type);
                if (convertedType !== null) {
                    const twitterAlert = { ...alert, type: convertedType } as TwitterAlert;

                    setAlerts((prev) => {
                        // Replace existing alert with same ID, or add new alert
                        const existingIndex = prev.findIndex(a => a.id === twitterAlert.id);
                        if (existingIndex >= 0) {
                            const updated = [...prev];
                            updated[existingIndex] = twitterAlert;
                            return updated;
                        }
                        return [twitterAlert, ...prev];
                    });
                }
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

    const contextValue: TwitterAlertContextType = {
        alerts,
        isConnected,
        connectionError,
        clearAlerts,
        removeAlert,
    };

    return (
        <TwitterAlertContext.Provider value={contextValue}>
            {children}
        </TwitterAlertContext.Provider>
    );
};

export const useTwitterAlerts = () => {
    const context = useContext(TwitterAlertContext);
    if (!context) {
        throw new Error("useTwitterAlerts must be used within a TwitterAlertProvider");
    }
    return context;
};

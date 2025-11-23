import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";
import type { TwitterAlert } from "../../models/Alert";
import { mockTwitterAlerts, generateRandomTwitterAlert } from "../../mocks/mockData";

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
  const [alerts, setAlerts] = useState<TwitterAlert[]>(mockTwitterAlerts);
  const [isConnected] = useState(true); // Always connected in demo mode
  const [connectionError] = useState<string | null>(null);

  const clearAlerts = () => setAlerts([]);

  const removeAlert = (alertId: string) => {
    setAlerts((prev) => prev.filter(alert => alert.id !== alertId));
  };

  // Simulate new alerts coming in periodically (every 30-60 seconds)
  useEffect(() => {
    const interval = setInterval(() => {
      const newAlert = generateRandomTwitterAlert();
      setAlerts((prev) => [newAlert, ...prev]);
    }, Math.random() * 30000 + 30000); // Random between 30-60 seconds

    return () => clearInterval(interval);
  }, []);

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

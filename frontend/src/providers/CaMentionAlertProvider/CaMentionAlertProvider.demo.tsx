import {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from "react";
import type { CaMentionAlert } from "../../models/Alert";
import { mockCaMentionAlerts } from "../../mocks/mockData";

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
  const [alerts, setAlerts] = useState<CaMentionAlert[]>(mockCaMentionAlerts);
  const [isConnected] = useState(true); // Always connected in demo mode
  const [connectionError] = useState<string | null>(null);

  const clearAlerts = () => setAlerts([]);

  const removeAlert = (alertId: string) => {
    setAlerts((prev) => prev.filter(alert => alert.id !== alertId));
  };

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

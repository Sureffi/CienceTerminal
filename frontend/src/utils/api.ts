/**
 * Utility functions for authenticated API requests
 */

/**
 * Performs an authenticated fetch request with JWT token
 * @param url - The URL to fetch
 * @param getToken - Function to retrieve the access token
 * @param options - Standard fetch options
 * @returns Promise<Response>
 */
export const authenticatedFetch = async (
  url: string,
  getToken: () => Promise<string>,
  options: RequestInit = {}
): Promise<Response> => {
  try {
    const token = await getToken();

    console.log('authenticatedFetch - Token retrieved:', token ? `${token.substring(0, 20)}...` : 'NO TOKEN');

    if (!token) {
      throw new Error('No access token available');
    }

    return fetch(url, {
      ...options,
      headers: {
        ...options.headers,
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });
  } catch (error) {
    console.error('authenticatedFetch - Error getting token:', error);
    throw error;
  }
};

/**
 * Helper to get SignalR access token factory function
 * @param getToken - Function to retrieve the access token
 * @returns Function that returns a promise of the token
 */
export const createSignalRTokenFactory = (
  getToken: () => Promise<string>
) => {
  return async () => await getToken();
};

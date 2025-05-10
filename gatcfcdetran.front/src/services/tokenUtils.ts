export function decodeJWT(token: string | null): object | null {
    if (!token) return null;
  
    const parts = token.split('.');
    if (parts.length !== 3) {
      console.error("Token inválido");
      return null;
    }
  
    try {
      const payload = parts[1];
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const decodedPayload = JSON.parse(
        decodeURIComponent(
          atob(base64)
            .split('')
            .map(function (c) {
              return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            })
            .join('')
        )
      );
  
      return decodedPayload;
    } catch (e) {
      console.error("Erro ao decodificar o token:", e);
      return null;
    }
  }
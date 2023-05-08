import {AuthService}  from './AuthService';

export function authHeader() {
    // return authorization header with jwt token
    const currentUser = AuthService.getCurrentUser();
    if (currentUser && currentUser.token) {
        return { Authorization: `Bearer ${currentUser.token}` };
    } else {
        return {};
    }
}
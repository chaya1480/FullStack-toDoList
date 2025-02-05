import React, { useState } from 'react';
import { login, register } from './authService';

function AuthRegistrationLogin() {
    const [isLoginMode, setIsLoginMode] = useState(true); // מצב כניסה/הרשמה
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (isLoginMode) {
                await login(username, password);
                alert('Login successful');
                window.location.href = '/tasks';
            } else {
                await register(username, password); 
                alert('Registration successful, redirecting to tasks...');
            }
        } catch (error) {
            alert(
                isLoginMode
                    ? 'Login failed: ' + (error.response?.data || error.message)
                    : 'Registration failed: ' + (error.response?.data || error.message)
            );
        }
    };
    
    return (
        <div style={{ maxWidth: '400px', margin: '0 auto', textAlign: 'center' }}>
            <h1>{isLoginMode ? 'Login' : 'Register'}</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                <br />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <br />
                <button type="submit">{isLoginMode ? 'Login' : 'Register'}</button>
            </form>
            <p>
                {isLoginMode
                    ? "Don't have an account? "
                    : 'Already have an account? '}
                <button onClick={() => setIsLoginMode(!isLoginMode)} style={{ background: 'none', border: 'none', color: 'blue', cursor: 'pointer', textDecoration: 'underline' }}>
                    {isLoginMode ? 'Register' : 'Login'}
                </button>
            </p>
        </div>
    );
}

export default AuthRegistrationLogin;

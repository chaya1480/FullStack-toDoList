// import React, { useEffect, useState } from 'react';
// import service from './service.js';

// function TasksPage() {
//     const [newTodo, setNewTodo] = useState("");
//     const [todos, setTodos] = useState([]);

//     async function getTodos() {
//         try {
//             const todos = await service.getTasks();
//             setTodos(todos);
//         } catch (error) {
//             console.error("Error fetching tasks:", error);
//         }
//     }

//     async function createTodo(e) {
//         e.preventDefault();
//         await service.addTask(newTodo);
//         setNewTodo("");
//         await getTodos();
//     }

//     async function updateCompleted(todo, isComplete) {
//         await service.setCompleted(todo.id, isComplete);
//         await getTodos();
//     }

//     async function deleteTodo(id) {
//         await service.deleteTask(id);
//         await getTodos();
//     }
//     useEffect(() => {
//         getTodos();
//     }, []);
//     return (
//         <section className="todoapp">
//             <header className="header">
//                 <h1>todos</h1>
//                 <form onSubmit={createTodo}>
//                     <input className="new-todo" placeholder="!אין דבר שאי אפשר להשלים – פשוט תתחילי" value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
//                 </form>
//             </header>
//             <section className="main" style={{ display: "block" }}>
//                 <ul className="todo-list">
//                     {todos.map(todo => (
//                         <li key={todo.id} className={todo.isComplete ? "completed" : ""}>
//                             <input
//                                 type="checkbox"
//                                 checked={todo.isComplete}
//                                 onChange={(e) => updateCompleted(todo, e.target.checked)}
//                                 className="toggle"
//                             />
//                             <span>{todo.name}</span>
//                             <button className="destroy" onClick={() => deleteTodo(todo.id)}>✂️</button>
//                         </li>
//                     ))}
//                 </ul>
//             </section>
//         </section>
//     );
// }

// export default TasksPage;

import React, { useEffect, useState } from 'react';
import service from './service.js';
import { logout } from './authService';

function TasksPage() {
    const [newTodo, setNewTodo] = useState("");
    const [todos, setTodos] = useState([]);
    const [username, setUsername] = useState("");

    // useEffect(() => {
    //     const token = localStorage.getItem("token");
    //     if (token) {
    //         const payload = JSON.parse(atob(token.split('.')[1]));
    //         setUsername(payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
    //     }
    //     getTodos();
    // }, []);
    useEffect(() => {
        const token = localStorage.getItem("token");
    
        if (token && token !== "undefined" && token !== "null") {
            // try {
                const parts = token.split('.');
                if (parts.length !== 3) {
                    throw new Error("Invalid JWT format");
                }
                const payload = JSON.parse(atob(parts[1]));
                setUsername(payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
                console.log("username:" + username)
            // } catch (error) {
            //     console.error("Error decoding token:", error);
            //     setUsername(""); 
            // }
        }
    
        getTodos();
    }, []);
    
    // async function getTodos() {
    //     try {
    //         const todos = await service.getTasks();
    //         setTodos(todos);
    //     } catch (error) {
    //         console.error("Error fetching tasks:", error);
    //     }
    // }
    async function getTodos() {
        try {
            const todos = await service.getTasks();
            console.log("Fetched todos:", todos); // בדיקת הערך המתקבל
            setTodos(Array.isArray(todos) ? todos : []);
        } catch (error) {
            console.error("Error fetching tasks:", error);
            setTodos([]); // מניעת קריסה במקרה של שגיאה
        }
    }
    
    async function createTodo(e) {
        e.preventDefault();
        await service.addTask(newTodo);
        setNewTodo("");
        await getTodos();
    }

    async function updateCompleted(todo, isComplete) {
        await service.setCompleted(todo.id, isComplete);
        await getTodos();
    }

    async function deleteTodo(id) {
        await service.deleteTask(id);
        await getTodos();
    }

    const handleLogout = () => {
        logout();
        window.location.href = '/';
    };

    return (
        <section className="todoapp">
            <header className="header">
                <h1>Welcome, {username}</h1>
                <form onSubmit={createTodo}>
                    <input className="new-todo" placeholder='!אין דבר שא"א להשלים – פשוט תתחילי' value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
                </form>
            </header>
            <section className="main" style={{ display: "block" }}>
                <ul className="todo-list">
                    {todos.map(todo => (
                        <li key={todo.id} className={todo.isComplete ? "completed" : ""}>
                            <input
                                type="checkbox"
                                checked={todo.isComplete}
                                onChange={(e) => updateCompleted(todo, e.target.checked)}
                                className="toggle"
                            />
                            <span>{todo.name}</span>
                            <button className="destroy" onClick={() => deleteTodo(todo.id)}>✂️</button>
                        </li>
                    ))}
                </ul>
            </section>
            <button onClick={handleLogout} className="logout-button" style={{ marginBottom: '20px' }}>Logout</button>
        </section>
    );
}

export default TasksPage;

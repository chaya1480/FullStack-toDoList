import React, { useEffect, useState } from 'react';
import { useNavigate, Routes, Route, Navigate } from 'react-router-dom';
import service from './service.js';
import AuthRegistrationLogin from './AuthRegistrationLogin.js';
import TasksPage from './TasksPage.js';

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/');
    }
    else {
      getTodos();
    }
  }, []);

  function isAuthenticated() {
    const token = localStorage.getItem("token");
    return token !== null;
  }


  async function getTodos() {
    const todos = await service.getTasks();
    setTodos(todos);
  }

  // async function createTodo(e) {
  //   e.preventDefault();
  //   await service.addTask(newTodo);
  //   setNewTodo("");
  //   await getTodos();
  // }

  // async function updateCompleted(todo, isComplete) {
  //   await service.setCompleted(todo.id, isComplete);
  //   await getTodos();
  // }

  // async function deleteTodo(id) {
  //   await service.deleteTask(id);
  //   await getTodos();
  // }

  return (
    <Routes>
      <Route path="/" element={isAuthenticated() ? <Navigate to="/tasks" /> : <AuthRegistrationLogin />} />
      <Route path="/login" element={<AuthRegistrationLogin />} />
      <Route path="/tasks" element={isAuthenticated() ? <TasksPage /> : <Navigate to="/" />} />
    </Routes>
  );
}

export default App;

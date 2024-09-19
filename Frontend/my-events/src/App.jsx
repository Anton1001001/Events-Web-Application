import React from 'react';
import { ChakraProvider, Box } from '@chakra-ui/react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Header from './Components/Header';
import EventList from './Pages/EventList';
import AuthPage from './Pages/AuthPage';
import AdminPage from './Pages/AdminPage';
import EventDetail from './Pages/EventDetail'; 
import EventRegistration from './Pages/EventRegistration';
import MyEvents from './Pages/MyEvents';
import Footer from './Components/Footer';
import EventUsersPage from './Pages/EventUsersList';

function App() {
  return (
    <ChakraProvider>
      <Box display="flex" flexDirection="column" minHeight="100vh">
        <Router>
          <Header />
          <Box flexGrow={1}>
            <Routes>
              <Route path="/" element={<Navigate to="/events" />} />
              <Route path="/events" element={<EventList />} />
              <Route path="/auth" element={<AuthPage />} />
              <Route path="/admin" element={<AdminPage />} />
              <Route path="/event/:eventId/users" element={<EventUsersPage />} />
              <Route path="/event/:id" element={<EventDetail />} />
              <Route path="/event/:id/register" element={<EventRegistration />} />
              <Route path="/my-events" element={<MyEvents />} />
            </Routes>
          </Box>
          <Footer />
        </Router>
      </Box>
    </ChakraProvider>
  );
}

export default App;

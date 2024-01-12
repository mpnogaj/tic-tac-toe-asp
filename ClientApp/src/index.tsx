import { Base } from '@/endpoints';
import axios from 'axios';
import React from 'react';
import { createRoot } from 'react-dom/client';

import App from './App';

const container = document.getElementById('app');
const root = createRoot(container!);

axios.defaults.withCredentials = true;
axios.defaults.baseURL = Base;

root.render(
	<React.StrictMode>
		<App />
	</React.StrictMode>
);

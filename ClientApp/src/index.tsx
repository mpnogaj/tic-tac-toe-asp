import React from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import axios from "axios";
import {Base} from "@/endpoints";

const container = document.getElementById('app');
const root = createRoot(container!);

axios.defaults.withCredentials = true;
axios.defaults.baseURL = Base;

root.render(
	<React.StrictMode>
		<App />
	</React.StrictMode>
);

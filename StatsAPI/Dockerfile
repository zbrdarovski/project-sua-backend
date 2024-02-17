FROM node:16.14.2

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 3005

CMD ["node", "app.js"]
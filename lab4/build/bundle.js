/******/ (() => { // webpackBootstrap
/******/ 	var __webpack_modules__ = ({

/***/ "./src/Autor.ts":
/*!**********************!*\
  !*** ./src/Autor.ts ***!
  \**********************/
/***/ (() => {

class Autor {
    get imie() { return this._imie; }
    get nazwisko() { return this._nazwisko; }
    get email() { return this._email; }
    constructor(imie, nazwisko, email) {
        if (!imie || imie.trim() === "") {
            throw new Error("Pole 'imie' jest wymagane.");
        }
        if (!nazwisko || nazwisko.trim() === "") {
            throw new Error("Pole 'nazwisko' jest wymagane.");
        }
        if (!email || email.trim() === "") {
            throw new Error("Pole 'email' jest wymagane.");
        }
        if (!Autor.czyPoprawnyEmail(email)) {
            throw new Error(`Niepoprawny adres e-mail: ${email}`);
        }
        this._imie = imie.trim();
        this._nazwisko = nazwisko.trim();
        this._email = email.trim();
    }
    static czyPoprawnyEmail(email) {
        const wzorzecEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return wzorzecEmail.test(email);
    }
}


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/compat get default export */
/******/ 	(() => {
/******/ 		// getDefaultExport function for compatibility with non-harmony modules
/******/ 		__webpack_require__.n = (module) => {
/******/ 			var getter = module && module.__esModule ?
/******/ 				() => (module['default']) :
/******/ 				() => (module);
/******/ 			__webpack_require__.d(getter, { a: getter });
/******/ 			return getter;
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry needs to be wrapped in an IIFE because it needs to be in strict mode.
(() => {
"use strict";
/*!**********************!*\
  !*** ./src/index.ts ***!
  \**********************/
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _Autor__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./Autor */ "./src/Autor.ts");
/* harmony import */ var _Autor__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(_Autor__WEBPACK_IMPORTED_MODULE_0__);

const autorzy = [
    new _Autor__WEBPACK_IMPORTED_MODULE_0__.Autor("Adam", "Kowalski", "adam.kowalski@example.com"),
    new _Autor__WEBPACK_IMPORTED_MODULE_0__.Autor("Ewa", "Nowak", "ewa.nowak@example.com"),
    new _Autor__WEBPACK_IMPORTED_MODULE_0__.Autor("Piotr", "Zieliński", "piotr.zielinski@example.com"),
];
// funkcja generująca tabelę HTML
function generujTabeleAutorow(autorzy) {
    let html = `
        <table border="1" cellpadding="5" cellspacing="0">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Imię</th>
                    <th>Nazwisko</th>
                    <th>Email</th>
                </tr>
            </thead>
            <tbody>
    `;
    for (const a of autorzy) {
        html += `
            <tr>
                <td>${a.id}</td>
                <td>${a.imie}</td>
                <td>${a.nazwisko}</td>
                <td>${a.email}</td>
            </tr>
        `;
    }
    html += `</tbody></table>`;
    return html;
}
// --- wyświetlenie w przeglądarce ---
document.body.innerHTML = `
    <h1>Lista autorów</h1>
    ${generujTabeleAutorow(autorzy)}
`;

})();

/******/ })()
;
//# sourceMappingURL=bundle.js.map
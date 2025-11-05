/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/Autor.ts":
/*!**********************!*\
  !*** ./src/Autor.ts ***!
  \**********************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   Autor: () => (/* binding */ Autor)
/* harmony export */ });
class Autor {
    get imie() {
        return this._imie;
    }
    get nazwisko() {
        return this._nazwisko;
    }
    get email() {
        return this._email;
    }
    constructor(imie, nazwisko, email) {
        if (!imie || !nazwisko || !email) {
            throw "Wszystkie pola są wymagane.";
        }
        if (!this.isValid(email)) {
            throw "Niepoprawny email.";
        }
        this._imie = imie;
        this._nazwisko = nazwisko;
    }
    isValid(email) {
        const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return emailRegex.test(email);
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
// This entry needs to be wrapped in an IIFE because it needs to be isolated against other modules in the chunk.
(() => {
/*!**********************!*\
  !*** ./src/index.ts ***!
  \**********************/
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _Autor__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./Autor */ "./src/Autor.ts");

// Przykładowa lista autorów
const autorzy = [
    new _Autor__WEBPACK_IMPORTED_MODULE_0__.Autor("Jan", "Kowalski", "jan.kowalski@example.com"),
    new _Autor__WEBPACK_IMPORTED_MODULE_0__.Autor("Anna", "Nowak", "anna.nowak@example.com"),
    new _Autor__WEBPACK_IMPORTED_MODULE_0__.Autor("Piotr", "Zieliński", "piotr.zielinski@example.com")
];
// Funkcja generująca tabelę HTML
function generujTabeleAutora(autorzy) {
    let tabelaHTML = `
    <table border="1">
      <thead>
        <tr>
          <th>Imię</th>
          <th>Nazwisko</th>
          <th>Email</th>
        </tr>
      </thead>
      <tbody>`;
    // Generowanie wierszy tabeli na podstawie autorów
    autorzy.forEach((autor) => {
        tabelaHTML += ` 
      <tr>
        <td>${autor.imie}</td>
        <td>${autor.nazwisko}</td>
        <td>${autor.email}</td>
      </tr>`;
    });
    tabelaHTML += `
      </tbody>
    </table>`;
    return tabelaHTML;
}
// Dodanie tabeli HTML do strony (przykład dla przeglądarki)
document.getElementById("tabelaAutorow").innerHTML = generujTabeleAutora(autorzy);

})();

/******/ })()
;
//# sourceMappingURL=bundle.js.map
import React, { useState } from "react";

// Drugi komponent
const DrugiKomponent: React.FC = () => {
  return <h2>Witaj! To jest drugi komponent.</h2>;
};

// Typ propsów dla pierwszego komponentu
interface PierwszyKomponentProps {
  onClick: () => void;
}

// Pierwszy komponent
const PierwszyKomponent: React.FC<PierwszyKomponentProps> = ({ onClick }) => {
  return (
    <div>
      <h1>Pierwszy komponent</h1>
      <button onClick={onClick}>
        Pokaż drugi komponent
      </button>
    </div>
  );
};


// Główny komponent aplikacji
const App: React.FC = () => {
  const [pokazDrugi, setPokazDrugi] = useState<boolean>(false);

  return (
    <div>
      <PierwszyKomponent onClick={() => setPokazDrugi(true)} />
        
      {pokazDrugi ? (<DrugiKomponent />) : (
        <></>
      )}
    </div>
  );
};

export default App;

import { useState } from "react";

export default function First () {
    type LinkItem = {
        id: number;
        name: string;
        url: string;
    };

    const links: LinkItem[] = [
        { id: 1, name: "Google", url: "https://www.google.com" },
        { id: 2, name: "YouTube", url: "https://www.youtube.com" },
        { id: 3, name: "GitHub", url: "https://github.com" },
    ];

const [selectedLinks, setSelectedLinks] = useState<number[]>([]);

const handleClick = (id: number) => {
    setSelectedLinks((prev) =>
      prev.includes(id)
        ? prev.filter((linkId) => linkId !== id)
        : [...prev, id]
    );
  };

return (
    <div>
      <h2>Ulubione linki</h2>

      <ul>
        {links.map((link) => (
          <li key={link.id}>
            <a
              href={link.url}
              target="_blank"
              onClick={(e) => {
                e.preventDefault();
                handleClick(link.id);
              }}
              style={{
                color: selectedLinks.includes(link.id)
                  ? "red"
                  : "blue",
                cursor: "pointer",
                textDecoration: "underline",
              }}
            >
              {link.name}
            </a>
          </li>
        ))}
      </ul>
    </div>
);
}
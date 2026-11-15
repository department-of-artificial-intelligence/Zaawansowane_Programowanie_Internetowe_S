import { Autor } from './author.ts';
import { Blog } from './blog.ts';
import { Artykul } from './article.ts';
import { Komentarz } from './comment.ts';
import { CreateAutorDTO, AutorListItemDTO } from './dtos.ts';
import { AutorUpdater } from './AuthorUpdater.ts';
import { AutorExporter, JsonFormatter } from './AuthorExporter.ts';
import { CounterApp } from './counter.ts';
import { AutorTransformer} from './AuthorTranformer.ts';
import { Tree, TreeNode } from './Tree.js';

async function bootstrap(): Promise<void> {
  const counterRoot = document.getElementById('counter-app');
  if (counterRoot) {
    new CounterApp(counterRoot, 0);
  }

  const directoryRoot = document.getElementById('directory-app');
  if (directoryRoot) {
    const directoryApp = new AutorTransformer(directoryRoot, './dane.json');
    await directoryApp.init();

    directoryApp.wyswietlAutoraPoId(1);   
    directoryApp.wyswietlAutoraPoId(999); 
  }

  const nowyAutorDto: CreateAutorDTO = {
    imie: 'Marek',
    nazwisko: 'Zieliński',
    email: 'marek@example.com',
  };

  const autor = new Autor(nowyAutorDto.imie, nowyAutorDto.nazwisko, nowyAutorDto.email);
  AutorUpdater.aktualizuj(autor, { email: 'm.zielinski@example.com' });

  const pozycjaNaLiscie: AutorListItemDTO = {
    imie: autor.imie,
    nazwisko: autor.nazwisko,
  };
  console.log('DTO na listę:', pozycjaNaLiscie);

  const exportBtn = document.getElementById('export-json-btn');
  exportBtn?.addEventListener('click', () => {
    const exporter = new AutorExporter(new JsonFormatter());
    console.log(exporter.export([autor]));
  });

  const blog = new Blog('Tech Blog TS', autor);
  const artykul = new Artykul('Wprowadzenie do TypeScript', 'Treść artykułu...');
  const komentarzGlowny = new Komentarz('Świetny wpis!', 'Dev123');
  const odpowiedz = new Komentarz('Dzięki!', 'Marek');

  komentarzGlowny.dodajOdpowiedz(odpowiedz);
  artykul.dodajKomentarz(komentarzGlowny);
  blog.dodajArtykul(artykul);

  const rootNode = new TreeNode<string>(`Blog: ${blog.nazwa}`);
  const artykulNode = rootNode.addChild(new TreeNode(`Artykuł: ${artykul.tytul}`));
  const komNode = artykulNode.addChild(
    new TreeNode(`Komentarz (${komentarzGlowny.nick}): ${komentarzGlowny.tresc}`)
  );
  komNode.addChild(new TreeNode(`Odpowiedź (${odpowiedz.nick}): ${odpowiedz.tresc}`));

  const tree = new Tree(rootNode);
  tree.traverseDFS((node, depth) => {
    console.log(`${'  '.repeat(depth)}- ${node.value}`);
  });
}

bootstrap().catch((err) => console.error('Błąd uruchomienia aplikacji:', err));
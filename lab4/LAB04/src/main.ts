import { Author } from "./models/Author";
import { Blog } from "./models/Blog";
import { Article } from "./models/Article";
import { Comment } from "./models/Comment";

const author = new Author(1, "John", "Smith", "john@example.com");

const blog = new Blog(1, "My Blog", author);

const article = new Article(1, "TypeScript Basics", "My first article");

const comment = new Comment("Cool", "User123");

const reply = new Comment("Thank you!", "John");

blog.addArticle(article);
article.addComment(comment);
comment.addReply(reply);


console.log(blog.name);
console.log(blog.author.firstName);
console.log(blog.getArticleTitles());
console.log(article.getComments());
console.log(comment.getReplies());